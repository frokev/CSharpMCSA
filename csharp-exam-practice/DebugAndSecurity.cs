using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.Serialization;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace csharp_exam_practice
{
    // Debug applications and implement security

    class DebugAndSecurity
    {
        static void Main(string[] args)
        {
            new SymAndAsymEncryption();
            Console.ReadKey();
        }
    }

    // Validate application input
    // Validate JSON data; choose the appropriate data collection type; 
    // manage data integrity; evaluate a regular expression to validate the input format; 
    // use built-in functions to validate data type and content

    // TODO: Validate input using regex, choose a data collection type

    class ValidateInput
    {

        public ValidateInput()
        {
            // Serialize JSON into an object so we can validate type and content of each member
            GetResponse<DateObject>(new Uri("http://date.jsontest.com/"), dateObj => {
                var members = dateObj.getDataMembers().Select(x => x.Name);

                Console.WriteLine("Members: \n" + string.Join("\n", members) + "\n");
                Console.WriteLine("Is object complete: " + dateObj.IsComplete() + "\n"); // Validate data is defined for each DataMember property
                Console.WriteLine($"Member values:\n{dateObj.Time}\n{dateObj.Milliseconds_since_epoch}\n{dateObj.Date}\n");
            });
        }

        [DataContract]
        internal class DateObject
        {
            [DataMember(Name = "time")]
            internal string Time { get; set; }

            [DataMember(Name = "milliseconds_since_epoch")]
            internal long? Milliseconds_since_epoch { get; set; }

            [DataMember(Name = "date")]
            internal string Date { get; set; }

            // Returns each property which is a data member
            // This way we can separate between serialized props and initialized props if needed
            internal PropertyInfo[] getDataMembers()
            {
                var allProps = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var dataMembers = new List<PropertyInfo>();
                foreach (var prop in allProps)
                {
                    bool isDataMember = prop.GetCustomAttribute(typeof(DataMemberAttribute)) != null;
                    if (isDataMember) dataMembers.Add(prop);
                }
                return dataMembers.ToArray();
            }

            // use built-in functions to validate data type and content
            // Here we use reflection to validate all DataMember properties have content.
            // We can do this by ensuring all members are nullable so we can compare against a single value.
            internal bool IsComplete()
            {
                foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {

                    bool isDataMember = prop.GetCustomAttribute(typeof(DataMemberAttribute)) != null;
                    bool isNull = prop.GetValue(this) == null;
                    if (!isDataMember) continue;
                    if (isNull) return false;
                }

                return true;
            }
        }

        private void GetResponse<T>(Uri uri, Action<T> callback) where T : class
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (o, a) =>
            {
                if (callback != null)
                {
                    // So we can serialize into a dynamic object
                    if (typeof(T) == typeof(object))
                    {
                        StreamReader reader = new StreamReader(a.Result);
                        string json = reader.ReadToEnd();
                        reader.Dispose();
                        callback(JsonConvert.DeserializeObject(json) as T);
                        return;
                    }

                    // So we can serialize into a schema
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    callback(ser.ReadObject(a.Result) as T);

                }
            };
            wc.OpenReadAsync(uri);
        }
    }

    // Perform symmetric and asymmetric encryption
    // Choose an appropriate encryption algorithm; 
    // manage and create certificates; implement key management; 
    // implement the System.Security namespace; hash data; encrypt streams
    class SymAndAsymEncryption
    {
        // Symmetric encryption - Same key for encryption and decryption
        // Symmetric encryption is faster and should be prefered
        // Therefore public key encryption is used to send an encrypted symmetric key for use in a session of data exchange.
        // A new pub/priv key should be generated at the start of each session for increased security.
        // Asymmetric encryption - Or public key encryption, means encrypting and decrypting use different keys. (public key for encrypting)
        // By crafting a key pair we mean that a public and private key need to be crafted together to work
        // Public key encryption rely on the message being smaller than the key!

        public SymAndAsymEncryption()
        {
            
        }

        public void EncryptDecryptStreamTest()
        {
            // encrypt/decrypt streams; symmetric encryption (see bin->debug for test.txt output)
            var fs = new FileStream("test.txt", FileMode.Create, FileAccess.ReadWrite);
            RijndaelManaged rmCrypto = new RijndaelManaged(); // Encryption algorithm
            rmCrypto.GenerateIV(); // make IV
            rmCrypto.GenerateKey(); // make key

            // Encrypt and write
            var toEncrypt = "This will get encrypted";
            // Stream wrappers must have "leaveOpen" parm set true if we intend to dispose them and reuse the base stream
            var csw = new CryptoStream(fs, rmCrypto.CreateEncryptor(), CryptoStreamMode.Write, true);
            var sw = new StreamWriter(csw, Encoding.Default, 100, true);
            sw.WriteLine(toEncrypt);
            sw.Dispose();
            csw.Dispose();
            // Must reset stream position before we read. It will be at the end after we write to it.
            // Must flush all stream wrappers before we set new position (dispose will close and flush)
            fs.Position = 0;

            // Decrypt and read
            //var fsRead = new FileStream("test.txt", FileMode.Open, FileAccess.Read);
            var csr = new CryptoStream(fs, rmCrypto.CreateDecryptor(), CryptoStreamMode.Read, true);
            var sr = new StreamReader(csr, Encoding.Default, false, 100, true);
            char[] buff = new char[100];
            sr.Read(buff, 0, 100);
            string res = string.Join("", buff);
            Console.WriteLine("Decrypted text in test.txt to:\n{0}", res);

            csr.Dispose();
            sr.Dispose();
            fs.Dispose();
            //-----------------
        }

        public void HashDataTest()
        {
            // hash data
            byte[] hash = GenerateHash("Hello World");
            bool isValid = VerifyHash("Hello World", hash);
            string[] strarr = hash.Select(b => b.ToString()).ToArray();
            Console.WriteLine($"Hashed 'Hello World' to: {string.Join(" ", strarr)}\nVerification method: {isValid}\n");
            // ---------
        }

        public byte[] GenerateHash(string value)
        {
            SHA256Managed sha2 = new SHA256Managed();
            byte[] byteVal = Encoding.Unicode.GetBytes(value);
            byte[] hashedVal = sha2.ComputeHash(byteVal);

            return hashedVal;
        }

        public bool VerifyHash(string value, byte[] hash)
        {
            SHA256Managed sha2 = new SHA256Managed();
            byte[] byteVal = Encoding.Unicode.GetBytes(value);
            byte[] hashedVal = sha2.ComputeHash(byteVal);
            return hashedVal.SequenceEqual(hash);
        }

        // Public key encryption (asymmetric)
        public void RSAEncryptDecryptTest()
        {
            byte[] data = { 1, 2, 3, 4, 5, }; // This is what we're encrypting. (why does a byte array take a 32-bit signed integer which is 4 bytes?)
            using (var rsa = new RSACryptoServiceProvider(2048)) // Keysize of 2048 is recommended for increased security
            {
                // rsa instance generate a keypair on encryption if none is specified.
                byte[] encrypted = rsa.Encrypt(data, true); // encrypted message (OAEP padding adds security)
                Console.WriteLine("Encrypted byte array: " + string.Join(", ", encrypted));
                byte[] decrypted = rsa.Decrypt(encrypted, true); // decrypt message
                Console.WriteLine("Decrypted byte array: " + string.Join(", ", decrypted));
            }
        }

        // Generate and save keys to xml string
        public void RSAGenerateToXML()
        {
            using  (var rsa = new RSACryptoServiceProvider(2048))
            {
                // rsa will generate new keys on first call to ToXmlString if none is existing
                File.WriteAllText("PublicKeyOnly.xml", rsa.ToXmlString(false));
                File.WriteAllText("PublicPrivate.xml", rsa.ToXmlString(true));
                Console.WriteLine("Generated public key added to: PublicKeyOnly.xml");
                Console.WriteLine("Generated public and private key added to: PublicPrivate.xml");
            }
        }

        // Use existing keys
        public void RSAUseExistingTest()
        {
            byte[] data = Encoding.UTF8.GetBytes("Message to encrypt");
            string publicKeyOnly = File.ReadAllText("PublicKeyOnly.xml");
            string publicPrivate = File.ReadAllText("PublicPrivate.xml");
            byte[] encrypted, decrypted;
            using (var rsaPublicOnly = new RSACryptoServiceProvider())
            {
                rsaPublicOnly.FromXmlString(publicKeyOnly);
                encrypted = rsaPublicOnly.Encrypt(data, true);
                Console.WriteLine("Data encrypted using public key");
                // The next line would throw an exception because you need the private
                // key in order to decrypt:
                // decrypted = rsaPublicOnly.Decrypt (encrypted, true);
            }            using (var rsaPublicPrivate = new RSACryptoServiceProvider())
            {
                // With the private key we can successfully decrypt:
                rsaPublicPrivate.FromXmlString(publicPrivate);
                decrypted = rsaPublicPrivate.Decrypt(encrypted, true);
                Console.WriteLine("Data decrypted using private key");
            }

        }

        public void RSASignDataTest()
        {
            byte[] data = Encoding.UTF8.GetBytes("Message to sign");
            byte[] publicKey;
            byte[] signature;
            object hasher = SHA1.Create(); // Our chosen hashing algorithm.
                                           
            // Generate a new key pair, then sign the data with it:
            using (var publicPrivate = new RSACryptoServiceProvider())
            {
                signature = publicPrivate.SignData(data, hasher);
                publicKey = publicPrivate.ExportCspBlob(false); // get public key
            }

            // Create a fresh RSA using just the public key, then test the signature.
            using (var publicOnly = new RSACryptoServiceProvider())
            {
                publicOnly.ImportCspBlob(publicKey); // Use public key of the signer
                Console.Write(publicOnly.VerifyData(data, hasher, signature)); // True
                // Let's now tamper with the data, and recheck the signature:
                data[0] = 0;
                Console.Write(publicOnly.VerifyData(data, hasher, signature)); // False
                // The following throws an exception as we're lacking a private key:
                // signature = publicOnly.SignData(data, hasher);
            }
        }

        // We can make a certificate using "makecert" commmand from VS command prompt
        // Here I have made a certificate in a certification store called "demoCertStore" on the local machine using:
        // "makecert -n "CN=MyName" -ss demoCertStore -sr currentuser
        public void Certificates()
        {
            // Sign a message
            X509Store store = new X509Store("demoCertStore", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly); // Open certificate store

            X509Certificate2 cert = store.Certificates[0]; // Get a representation of our certificate
            RSACryptoServiceProvider rsa = cert.PrivateKey as RSACryptoServiceProvider; // Pass the private asym algo object to our RSA provider

            string messageToSign = "Hello World";
            byte[] msgInBytes = Encoding.ASCII.GetBytes(messageToSign);

            byte[] signature = rsa.SignData(msgInBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1); // Sign message
            Console.WriteLine("Signed message '{0}', signature:\n{1}", messageToSign, string.Join(" ", signature));

            // Validate data by checking the signature
            RSACryptoServiceProvider rsaDecryptor = cert.PublicKey.Key as RSACryptoServiceProvider;
            bool isDataValid = rsaDecryptor.VerifyData(msgInBytes, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            Console.WriteLine("Does this data has the correct signature? {0}\n", isDataValid);

            rsaDecryptor.Dispose();
            rsa.Dispose();
            cert.Dispose();
            store.Dispose();
        }
    }

    // Manage Assemblies
    // Version assemblies; sign assemblies using strong names; 
    // implement side-by-side hosting; put an assembly in the global assembly cache; 
    // create a WinMD assembly
    class ManageAssemblies
    {

    }
}

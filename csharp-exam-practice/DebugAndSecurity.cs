using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.Serialization;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Reflection;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Security.Cryptography;

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
            // hash data
            byte[] hash = GenerateHash("Hello World");
            bool isValid = VerifyHash("Hello World", hash);
            string [] strarr = hash.Select(b => b.ToString()).ToArray();
            Console.WriteLine($"Hashed 'Hello World' to: {string.Join(" ", strarr)}\nVerification method: {isValid}\n");
            // ---------

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
    }
}

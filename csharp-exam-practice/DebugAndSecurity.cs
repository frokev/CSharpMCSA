#define DIAGNOSTICS

using System;
using System.Diagnostics;
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
using System.Threading;

namespace csharp_exam_practice
{
    // ------------------------------Debug applications and implement security-------------------------------------

    public class DebugAndSecurity
    {
        //static void Main(string[] args)
        //{
        //    new DebugAnApplication().PerformancerMonitor();
        //    Console.ReadKey();
        //}
    }

    // Validate application input -------------------------------------------------------
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

    // Perform symmetric and asymmetric encryption -----------------------------------------------------------------------
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
            }

            using (var rsaPublicPrivate = new RSACryptoServiceProvider())
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

    class ManageAssemblies { } // Just for easy navigation in solution explorer
    // Manage Assemblies ------------------------------------------------------------

    // Version assemblies; sign assemblies using strong names; 
    // implement side-by-side hosting; put an assembly in the global assembly cache; 
    // create a WinMD assembly

    // Assembly information can be found in AssemblyInfo.cs,
    // Visual Studio generates this file when you build the assembly,
    // you can change it's content by going to project properties -> package


    // Version Assemblies

    // A version number can be changed in AssemblyInfo.cs
    // The runtime makes sure applications run only with the version they were built and tested with
    // This behaviour can be overridden by explicit version policy, in config files
    // An assembly version is expressed as 4 values: {Major}.{Minor}.{Build}.{Revision}
    // Major version: Incremented with new features or breaking changes.
    // Minor version: Incremented when smaller changes are made to the assembly.
    // Build number: Incremented with each build, used by server to identify changes
    // Revision number: Used to track patched versions in a production environment


    // Sign Assemblies

    // A strong named assembly is signed and validated using asymmetric encryption.
    // You can generate keys for signing an assembly using the "sn" command line command,
    // or sign the assembly in VS by going to sign tab in project properties.
    // When the assembly is signed, the manifest will contain the public key, which is used for validation.
    // Signing an assembly allows us to ensure it hasn't been tampered with.
    // It is necessary if you want to put it in the GAC (Global Assembly Cache)


    // Implement side-by-side hosting

    // Since assemblies in the GAC are strongly-named they can be uniquely identified,
    // this allows us to hold multiple versions of the same assembly.
    // This is called side-by-side hosting.
    // It allows applications to reference different version of the same assembly, hence preventing "dll hell".
    // If you need all applications that previously referenced the old assembly,
    // to reference the new assembly, you can use "assembly binding redirection".
    // This can be done trough a policy file with the name of the apps executable following ".config" (new assembly must have same publicKeyToken)


    // Put an assembly in the global assembly cache

    // The GAC allows an assembly to be visible among applications on the machine
    // Assemblies in the GAC is in a folder on the machine: windows folder -> assembly


    // Create a WinMD assembly

    // A WinMD assembly (WinRT component) allows windows applications to be deployed across different devices,
    // It can do this by exposing the API elements of windows trough WinMD files
    // We can create a WinMD assembly in C# by starting a new Windows Runtime Component project, from the "windows universal" tab.

    // ----------------------------------------------------------------------------------


    // -----------------------Debug an application ---------------------------------
    // Create and manage preprocessor directives; choose an appropriate build type; 
    // manage program database files (debug symbols)

    class DebugAnApplication {

        // Create and manage preprocessor directives

        // We can use preprocessor directives to tell the compiler things, 
        // like ignore compilation of certain instructions used in testing (conditional compilation).
        // Our diagnostics symbol must be defined at the top of the file "#define DIAGNOSTICS"
        // We can also define symbols in the project properties -> Build
        // #pragma directive is used to suppress warnings for a region of code: #pragma warning disable, #pragma warning enable
        public void ConditionalCompilation()
        {
            string test = "test";
            Console.WriteLine("I got compiled");
#if DIAGNOSTICS
            Console.WriteLine("I might have not been compiled {0}", test);
#endif
        }

        // We can prevent a method from being called using an attribute and our DIAGNOSTICS symbol
        // This will always be compiled, but our symbol decides if it will run
        [Conditional("DIAGNOSTICS")]
        public void ConditionalAttribute()
        {
            Console.WriteLine("DIAGNOSTICS is defined");
        }

        // Choose an appropriate build type

        // There are two debug types by default - build and debug
        // You can configure the behaviours for these under project properties
        // By default the release build makes optimizations and removes operations used for debugging
        // We can make a custom build using the Configuration Manager from the build mode dropdown in VS


        // Manage programming database files (debug symbols)

        // A program debug database file (symbol file) is produced in compilation.
        // The file output is of a .pdb extention.
        // It includes debugger information and mapping of source code to compiled statements.
        // The mapping includes all code symbols with mapping to their memory addresses, when the program runs.
        // the symbol file contains a GUID which is held in the executable, so we can only use that file with the executable.

        // You can use the "pdbcopy" tool to modify the the existing .pdb file
        // This will create a copy holding the same GUID which allows you to replace the other one.
        // You can use the tool to remove private symbols or a list of symbols, such that they are not mapped.

        // You can use a symbol server to provide information to the debugger in place of the symbol file.
        // This can be done trough VS in tools -> options -> debugging -> symbols.
        // Here you can select a new symbol file to be used when debugging, and manually use the MS Symbol Servers.
        // By default symbols for modules are loaded automatically (as needed) when debugging you can turn this off in the options.

        //--------------------------------------------------------------------------------------------------


        // ------------------------Implement diagnostics in an application ------------------------------
        // Implement logging and tracing; profiling applications; 
        // create and monitor performance counters; write to the event log


        // Implement logging and tracing

        // We can use the Debug.WriteLine and Debug.WriteLineIf (System.Diagnostics) to log messages, 
        // the statements will only be included in the debug build 

        // The Trace object (System.Diagnostics) is usually defined for production builds,
        // such that these will produce output in production builds.
        // The TraceInformation, TraceWarning and TraceError objects is used to specify the importance of the message.

        // By default Trace output is sent to the output window in VS, 
        // we can send this elsewhere by using the TraceListener object.
        // There are different types of TraceListener for sending data to console, text file and more.
        // We can inherit the class to send the output data anywhere we want.
        // EventLogTraceListener can be used to write to the event log.

        // We can use the TraceSource class to output more than just simple messages
        // Like event types, define values that represent an event and messages, even object references

        // We can write output based on the tracing level with the SourceSwitch class
        
        public void SourceSwitchTest()
        {
            TraceSource trace = new TraceSource("Tracer", SourceLevels.All);
            SourceSwitch ss = new SourceSwitch("Control", "Trace output");
            ss.Level = SourceLevels.Information;
            trace.Switch = ss;

            trace.TraceEvent(TraceEventType.Information, 10001, "info");
            trace.TraceEvent(TraceEventType.Verbose, 10002, "verbose");
            trace.TraceEvent(TraceEventType.Information, 10001, "info", new object[] { "yo", 15 });
            Trace.TraceWarning("waaarrrn");
            Trace.TraceError("errrr");
            trace.Flush();
            trace.Close();
        }

        // We can configure tracing using the application .config file

        // We can use the Stopwatch class to measure speed of operations
        // We can use VS diagnosic tools to measure performance of an app
        // Performance profiler provides profiling tools to check things like cpu usage, memory leaks etc.

        public void StopwatchTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool what = false;
            for (int i = 0; i < 1000; i++) Console.WriteLine(i);
            sw.Stop();
            Console.WriteLine("Time to print 1000 messages.\n{0} ms", sw.ElapsedMilliseconds);
        }

        // Assertions are used for statements you believe to be true,
        // if it's not, there will be an output with options to: abort, retry and ignore.


        // Create and monitor performance counters

        // We can use the perfmon tool to monitor the performance of a computer
        // It contains a large number of performance counters

        // A program can monitor performance counters like so:
        
        public void PerformancerMonitor()
        {
            PerformanceCounter processor = new PerformanceCounter(
                categoryName: "Processor Information", 
                counterName: "% Processor Time", 
                instanceName: "_Total");

            while (true)
            {
                Console.WriteLine("Processor time: {0}", processor.NextValue());
                Thread.Sleep(500);
                if (Console.KeyAvailable)
                    break;
            }
        }

        // You can create performance counters using PerformanceCounterCategory.Create


        // We can read and write to the event log using EventLog class.
    }
    //------------------------------------------------------------------------------

}

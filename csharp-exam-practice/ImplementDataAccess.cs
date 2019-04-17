using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace csharp_exam_practice
{
    // ---------------------------------Implement Data Access---------------------------------
    class ImplementDataAccess
    {
        static void Main(string[] args)
        {
            new ConsumeData().TestService(2);
            Console.ReadKey();
        }
    }

    // ---------------Perform I/O operations---------------
    // Read and write files and streams; 
    // read and write from the network by using classes in the System.Net namespace; 
    // implement asynchronous I/O operations

    // Read / Write from stream example can be found in DebugAndSecutity -> SymAndAsymEncryption -> EncryptDecryptStreamTest()
    // FileMode is used to indicate how a file is opened
    // FileAccess is used to indicate how it's used (Read, write or both)
    // We can use the Encoding class from system.text to convert between data types
    // The File class is a helper class which makes it easier to work with files

    // Handle stream exceptions: They should be wrapped in trycatch

    // You can get info from operating system on a file using FileInfo
    // You can set it to readonly or not, using Attributes property on FileInfo

    // Manage directories using Directory and DirectoryInfo classes
    // We can use the GetFiles property on DirectoryInfo to search for files, using search strings (*, ?)
    // Path class can be used to manage file paths


    // Read and write from the Network using classes in System.Net

    class NetworkRequests
    {

        // We can use the WebRequest class to manage web requests:
        public void WebRequestTest()
        {
            var webRequest = WebRequest.Create("https://www.microsoft.com");
            var webResponse = webRequest.GetResponse();

            using (var sr = new StreamReader(webResponse.GetResponseStream()))
            {
                string siteText = sr.ReadToEnd();
                Console.WriteLine(siteText);
            }
        }

        // We can do this without dealing with streams using the WebClient class:
        public async void WebClientTest()
        {
            var wc = new WebClient();
            var onResult = wc.DownloadStringTaskAsync("https://www.microsoft.com");

            await onResult.ContinueWith((res) =>
            {
                Console.WriteLine(res.Result);
            });
        }

        // HttpClient is similar to WebClient except it only provides async methods
        public async void HttpClientTest()
        {
            var hc = new HttpClient();
            var res = await hc.GetStringAsync("https://www.microsoft.com");
            Console.WriteLine(res);
        }
    }


    // Implement asynchronous I/O operations

    class AsyncIO
    {
        public void WriteFileAsyncTest()
        {
            string toWrite = "hello file";
            using (var fw = new FileStream("test.txt", FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fw))
                {
                    sw.WriteAsync(toWrite);
                }
            }
        }
    }
    // ------------------------------------------------

    // -------------------Consume data---------------------------
    // Retrieve data from a database; update data in a database; 
    // consume JSON and XML data; retrieve data by using web services

    // We have set up a local database in MySql Workbench, with a schema test, and table person
    class ConsumeData
    {
        private string dbConnectionString = "Server=(localdb)\\mssqllocaldb;Database=MySQL57;Uid=root;Pwd=password;";

        public void ReadWithSQL()
        {
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM test.person");
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string firstName = reader["first_name"].ToString();
                    string lastName = reader["last_name"].ToString();
                    Console.WriteLine("First name: {0}, Last name: {1}", firstName, lastName);
                }
            }
            // Parameters to avoid SQL injection:
            // string connection = "UPDATE whatever SET name=@name WHERE id=@id"
            // command.Parameters.AddWithValue(@name, userInput);
        }

        // See DebugAndSecurity > ValidateInput. Here we serialize json data into an object.

        //XML documents:

        string XMLDocument = "<? xml version =\" 1.0\" encoding =\" utf-16\"? >" +
            "< MusicTrack xmlns:xsi =\" http:// www.w3. org/ 2001/ XMLSchema-instance\" " +
            "xmlns:xsd =\" http:// www.w3. org/ 2001/ XMLSchema\" > " +
            "< Artist > Rob Miles </ Artist > " +
            "< Title > My Way </ Title > " +
            "< Length > 150 </ Length >" +
            "</ MusicTrack >";

        public void ReadXMLDoc()
        {
            using (StringReader stringReader = new StringReader(XMLDocument))
            {
                XmlTextReader reader = new XmlTextReader(stringReader);
                while (reader.Read())
                {
                    string description = string.Format(" Type:{0} Name:{1} Value:{2}", reader.NodeType.ToString(), reader.Name, reader.Value);
                    Console.WriteLine(description);
                }
            }
        }

        // Using XMLDom to extract data
        public void XmlDomTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XMLDocument);
            XmlElement rootElement = doc.DocumentElement;
            // make sure it is the right element 
            if (rootElement.Name != "MusicTrack")
            {
                Console.WriteLine(" Not a music track");

            }
            else
            {
                string artist = rootElement[" Artist"].FirstChild.Value;
                Console.WriteLine("", artist);
                string title = rootElement[" Title"].FirstChild.Value;
                Console.WriteLine(" Artist:{ 0} Title:{ 1}", artist, title);
            }
        }

        // Retrieve data using web services, Windows Communication Foundation (WCF)

        // To run this web service, we need to put it in a separate WCF project
        // Here is the web service:

        ////[ServiceContract]
        //public interface IJokeOfTheDayService
        //{
        //    //[OperationContract]
        //    string GetJoke(int jokeStrength);
        //}

        //public class JokeOfTheDayService : IJokeOfTheDayService
        //{
        //    public string GetJoke(int jokeStrength)
        //    {
        //        string result = "invalid strength";
        //        switch (jokeStrength)
        //        {
        //            case 0:
        //                result = "Knock knock.";
        //                break;
        //            case 1:
        //                result = "Hello fellow";
        //                break;
        //            case 2:
        //                result = "Biggest joke";
        //                break;
        //        }
        //        return result;
        //    }
        //}

        // Add the service to your project -> "add connected service". Then paste in the url you got from WCF Test tool
        public async void TestService(int jokeStrength)
        {
            var service = new JokeOfTheDayService.JokeOfTheDayServiceClient();
            string joke = await service.GetJokeAsync(jokeStrength);

            Console.WriteLine("Joke received: " + joke);
        }
    } //-------------------------------------------------------------------------


    // -----------Query and manipulate data and objects by using LINQ--------------
    // Query data by using operators, including projection, join, group, take, skip, aggregate; 
    // create methodbased LINQ queries; query data by using query comprehension syntax; select data by using anonymous types; 
    // force execution of a query; read, filter, create, and modify data structures by using LINQ to XML

    // The LINQ library contains around 40 standard query operators
    class UsingLinQ
    {
        public void LinqTest()
        {
            string[] strArr = { "Tom", "Derrek", "Peter" };

            // Most query operators accept lambda expressions
            strArr.Where(n => n.Contains("er"));

            // We can use Query expression syntax
            IEnumerable<string> query = from n in strArr
                                        where n.Contains("To")
                                        select n;

            // Chaining query operators
            IEnumerable<string> query2 = strArr
                                        .Where(n => n.Contains("e")) // Query using operators
                                        .OrderBy(n => n.Length)
                                        .Select(n => n.ToUpper()); // Using projection (transforming an object)

            // SelectMany, we can return a sequence for each element, and get one resulting sequence
            strArr.SelectMany(n => n.Split());

            // Select using anonymous types
            var nameInfoSeq = strArr.Select(name => new { Name = name, name.Length, FirstChar = name[0] })
                              .ToArray(); // Force execution of query;

            foreach (var nameInfo in nameInfoSeq)
            {
                var name = nameInfo.Name;
                var length = nameInfo.Length;
                var fc = nameInfo.FirstChar;
            }

            // Aggregation operators return a scalar value (usually numeric)
            strArr.Count();
            strArr.Min();

            // Quantifiers return a bool value
            strArr.Contains("k");
            strArr.Any(n => n == null || n == "");

            // We can do concatination
            strArr.Concat(query2);
            strArr.Union(query2); // No duplicates

            // Deferred execution in Query operators
            var numbers = new List<int> { 1 };
            IEnumerable<int> query3 = numbers.Select(n => n * 10); // Build query
            numbers.Add(2); // Sneak in an extra element

            foreach (int n in query3) // Query operator will execute with each enumeration/iteration
                Console.Write(n + "|" + query3); // result: 10|20|

            // "Join" (inner join) a strategy used to match elements from two collections
            // Inner join: contains set of elements that share properties {1, 2, 3} and {one, three} will result in {{1, one}, {3, three}}

            //IQueryable<string> query4 =
            //                    from c in dataContext.Customers
            //                    join p in dataContext.Purchases on c.ID equals p.CustomerID
            //                    select c.Name + " bought a " + p.Description;


            // GroupJoin yields hierarchical results instead of flat results, grouped by each outer element (allows for left outer join)
            // Outer join, gets all elements from a set and include matching properties from the other set. 
            // e.g: { 1, 2, 3} and {one, three} will result in {{1, one}, {2, null}, {3, three}}

            // GroupBy is used to arrange identical data into groups e.g: {john, john} and {20, 30} results in {john, 50}

            // Custom Aggregation operation
            // Start with total, add n (name) and total separated by commas, return, total is now the returned value. 
            strArr.Aggregate("Inside strArr: ", (total, n) => total + n + ", ");

            // Take returns a specified number of elements from the start of a sequence 
            // TakeWhile returns elements until a specified condition is true
            // Take e.g:
            int[] grades = { 59, 82, 70, 56, 92, 98, 85 };

            IEnumerable<int> topThreeGrades =
                grades.OrderByDescending(grade => grade).Take(3);

            // Skip returns all elements except the specified number of elements at the start of a sequence.
            // SkipWhile skips elements until a specified condition is true, returns the remaining

            // Create method based LINQ queries. This is linq using fluent syntax instead of query expressions (a syntax build into c#)
            // Query comprehension syntax = linq query expressions


            // ----Read, filter, create, and modify data structures by using LINQ to XML----
            // We can use LINQ to parse XML

            // We use DOM to refer to the object model stored within the XML file.
            // C# has type definitions for an XML DOM, like XObject (root), XDocument (container root), XElement (container root) and XAttribute.

            XDocument fromWeb = XDocument.Load("http://albahari.com/sample.xml");
            XElement fromFile = XElement.Load(@"e:\media\somefile.xml");
            XElement config = XElement.Parse(
                        @"<configuration>
                             <client enabled='true'>
                                <timeout>30</timeout>
                             </client>
                         </configuration>");

            // We can traverse elements:
            foreach (XElement el in config.Elements())
                Console.WriteLine(el);

            // And:
            XElement client = config.Element("client");

            bool enabled = (bool)client.Attribute("enabled"); // Read attribute
            Console.WriteLine(enabled); // True
            client.Attribute("enabled").SetValue(!enabled); // Update attribute
            int timeout = (int)client.Element("timeout"); // Read element
            Console.WriteLine(timeout); // 30
            client.Element("timeout").SetValue(timeout * 2); // Update element
            client.Add(new XElement("retries", 3)); // Add new elememt

            // Saving
            client.Save("xmlfile.xml");

            // Nodes return all direct children, Elements return return only XElement types
            var nodes = client.Nodes();
            var eles = client.Elements();

            // We can cast XElement to get value
            int num = (int)client.Element("timeout");

            // We can do functional construction of the x-dom
            var bench = new XElement("bench",
                 new XElement("toolbox",
                     new XElement("handtool", "Hammer"),
                     new XElement("handtool", "Rasp")
                 ),
                 new XElement("toolbox",
                     new XElement("handtool", "Saw"),
                     new XElement("powertool", "Nailgun")
                 ),
                 new XComment("Be careful with the nailgun")
             );

            // Use LINQ to read - find the nailgun
            IEnumerable<string> findNailgun =
             from toolbox in bench.Elements()
             where toolbox.Elements().Any(tool => tool.Value == "Nailgun")
             select toolbox.Value; // Selects Saw and Nailgun

            // Filter with LINQ
            int x = bench.Elements().Where(e => e.Name == "toolbox").Count();


            // We can use LINQ to project into an x-dom, Create:

            //var customers =
            // new XElement("customers",
            //     from c in dataContext.Customers
            //     select
            //     new XElement("customer", new XAttribute("id", c.ID),
            //         new XElement("name", c.Name),
            //         new XElement("buys", c.Purchases.Count)
            //    )
            // );


        }
    } // ------------------------------------------------------------


    // ------------Serialize and deserialize data-------------------
    // Serialize and deserialize data by using: 
    // binary serialization
    // custom serialization 
    // XML Serializer 
    // JSON Serializer 
    // Data Contract Serializer
    class SerializeAndDeserialize
    {
        // See DebugAndSecurity->ValidateData for JSON serializer (DataContractJsonSerializer)

        // For Data Contract serializer
        [DataContract]
        public class Person
        {
            [DataMember] public string Name;
            [DataMember] public int Age;
        }

        // For Binary serializer/formatter
        [Serializable]
        public sealed class Dog
        {
            public string Name;
            public int Age;
        }

        // For XML Serializer
        public class Cat
        {
            public string Name;
            [XmlAttribute] // Give cat an attribute Age
            public int Age;
        }

        // For Custom Serialization
        [Serializable]
        class Horse : ISerializable
        {
            public string Name { get; set; }
            public Horse() { }

            public Horse(SerializationInfo info, StreamingContext context)
            {
                Name = (string)info.GetValue("props", typeof(string));
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("props", Name, typeof(string));
            }

            [OnSerializing]
            public void OnSerializing()
            {
                Console.WriteLine("Serializing started");
            }
        }

        public void DataContractSerializer()
        {
            Person p = new Person { Name = "Stacey", Age = 30 };
            var ds = new DataContractSerializer(typeof(Person));

            using (Stream s = File.Create("person.xml"))
                ds.WriteObject(s, p); // Serialize

            Person p2;
            using (Stream s = File.OpenRead("person.xml"))
                p2 = (Person)ds.ReadObject(s); // Deserialize

            Console.WriteLine(p2.Name + " " + p2.Age); // Stacey 30
        }

        public void BinarySerializer()
        {
            Dog d = new Dog() { Name = "Noa", Age = 25 };
            IFormatter formatter = new BinaryFormatter();
            using (FileStream s = File.Create("serialized.bin"))
                formatter.Serialize(s, d); // Serialize

            using (FileStream s = File.OpenRead("serialized.bin"))
            {
                Dog d2 = (Dog)formatter.Deserialize(s); // Deserialize
                Console.WriteLine(d2.Name + " " + d2.Age);
            }
        }

        public void XMLSerializer()
        {
            Cat c = new Cat();
            c.Name = "Stacey"; c.Age = 30;

            XmlSerializer xs = new XmlSerializer(typeof(Person));
            using (Stream s = File.Create("cat.xml"))
                xs.Serialize(s, c); // Serialize

            Cat c2;
            using (Stream s = File.OpenRead("cat.xml"))
                c2 = (Cat)xs.Deserialize(s); // Deserialize
            Console.WriteLine(c2.Name + " " + c2.Age); // Stacey 30
        }

        public void CustomSerializer()
        {
            Horse h = new Horse();
            h.Name = "Jimmy";

            // Serialize
            Stream s = File.Create("Horse");
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(s, h);
            s.Dispose();

            // Deserialize
            Stream s2 = File.OpenRead("Horse");
            Horse h2 = (Horse) formatter.Deserialize(s2);
        }

    }// ----------------------------------------------------


    // ---------Store data in and retrieve data from collections---------
    // Store and retrieve data by using dictionaries, arrays, lists, sets, and queues; choose a collection type; 
    // initialize a collection; add and remove items from a collection; 
    // use typed vs. non-typed collections; implement custom collections; implement collection interfaces

    class ManageCollections
    {

        class CustomCollection<T> : ICollection<T>
        {
            List<T> list = new List<T>();

            public int Count => list.Count();

            public bool IsReadOnly => false;

            public void Add(T item)
            {
                list.Add(item);
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(T item)
            {
                return list.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public bool Remove(T item)
            {
                return list.Remove(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }
        }

        public void Arrays()
        {
            int[] numbers = new int[] { 1, 2, 3 };
            int[] numers2 = new int[3];

            // Multi-dimentional
            int[,] numarr = new int[3,3] // row, col
            {
                { 1, 2, 3 },
                { 1, 2, 3 },
                { 1, 2, 3 }
            };

            int[][] jaggedArray = new int[][] // rows of different lenghts
            {
                new int[] {1, 2, 3, 4},
                new int[] {1, 2, 3},
                new int[] {1, 2}
            };
        }

        public void Lists()
        {
            var al = new ArrayList(); // Dynamic length, various types
            al.Add(1);
            al.Add("");

            var strli = new List<string>(); // Dynamic length, specified type
            strli.Add("");
        }

        public void Dictionary()
        {
            var dic = new Dictionary<string, string>(); // Use keys to access values
            dic.Add("key", "value");

            string val = dic["key"];
        }

        public void Set()
        {
            var hs = new HashSet<int>(); // Unordered collection
            hs.Add(1);
            
        }

        public void Queue()
        {
            var q = new Queue<string>();
            q.Enqueue("hey"); // First in first out
            q.Enqueue("yo");
            q.Dequeue(); // hey is out
        }

        public void Stack()
        {
            var stack = new Stack<string>();
            stack.Push("hey");
            stack.Push("yo"); // Last in first out
            stack.Pop(); // yo is out
        }
    }
}

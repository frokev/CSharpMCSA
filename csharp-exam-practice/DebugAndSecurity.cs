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

namespace csharp_exam_practice
{
    // Debug applications and implement security

    class DebugAndSecurity
    {
        static void Main(string[] args)
        {
            new ValidateInput();
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
            // Here we use reflection to validate all properties have content.
            // We can do this by ensuring all members are nullable.
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

    }
}

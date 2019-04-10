using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace csharp_exam_practice
{
    // ---------------------------------Implement Data Access---------------------------------
    class ImplementDataAccess
    {
        static void Main(string[] args)
        {
            new AsyncIO().WriteFileAsyncTest();
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

            await onResult.ContinueWith((res) => {
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

    class ConsumeData
    {
        public void ReadWithSQL()
        {
            
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CallExternalAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectToAPI();
            Console.ReadLine();
        }

        public static void ConnectToAPI ()
        {
            // Creates the WebRequest object using the uri string (in this case, a get request)
            string uri = String.Format("https://127.0.0.1:8904/api/cardholders");
            WebRequest requestObjGet = WebRequest.Create(uri);
            // requestObjGet.Credentials = new NetworkCredential("USERNAME","PASSWORD");

            // Adds API key as a header to the request. Note, must also append 'GGL-API-KEY' to the start
            requestObjGet.Headers.Add(HttpRequestHeader.Authorization.ToString(), "ADD-API-KEY XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX");

            // Allows you to bypass "AuthenticationException: The remote certificate is invalid according to the validation procedure." error. Don't to do this in production code...
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {return true;};

            requestObjGet.Method = "GET";

            // Creates an 'HttpWebResponse' object to handle the response from the server
            HttpWebResponse responseObjGet = null;
            responseObjGet = (HttpWebResponse)requestObjGet.GetResponse();

            // String to hold API request response data
            string strresulttest = null;

            // Reads data to string from response object and prints it to console
            using (Stream stream = responseObjGet.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                strresulttest = sr.ReadToEnd();
                Console.WriteLine(strresulttest);
                sr.Close();
            }
        }
    }
}

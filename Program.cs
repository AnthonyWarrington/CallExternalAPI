using System;
using System.IO;
using System.Linq;
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
            // Here we are just reading the key/thumbPrint from .txt files as string values
            string apiKey = File.ReadAllText(@"C:\Users\AnthonyW\Documents\APIkey.txt");
            string thumbPrint = File.ReadAllText(@"C:\Users\AnthonyW\Documents\Thumbprint.txt");

            // Creates the 'clientRequest' object using the uri string (in this case, a get request)
            string uri = String.Format("https://127.0.0.1:8904/api/");
            HttpWebRequest clientRequest = (HttpWebRequest)WebRequest.Create(uri);

            // Adds API key as a header to the request. Note, must also append 'GGL-API-KEY' to the start
            // API is in format "GGL-API-KEY XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX"
            clientRequest.Headers.Add(HttpRequestHeader.Authorization.ToString(), apiKey);

            // Adds certificate to request by passing certificate thumbprint to the 'GetX509Certificate' method
            clientRequest.ClientCertificates.Add(GetX509Certificate(thumbPrint));                                                                

            /* If "Could not establish trust relationship for the SSL/TLS Secure Channel...
             Authentication Exception: The remote certificate is invalid according to the validation procedure"
             is thrown, add 'ServicePointManager.ServerCertificateValidationCallback' as shown below...
             This allows you to bypass the Authentication Exception. Do not do this in production code... */
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s,
                X509Certificate certificate, 
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
                { return true; };

            // Although we aren't making a 'GET' request, this is still the method used to retreive a menu of
            // all API item types - e.g. accessZones, alarms, alarmZones, cardholders... 
            clientRequest.Method = "GET";

            // Creates an 'HttpWebResponse' object to handle the response from the server
            HttpWebResponse serverResponse = null;
            serverResponse = (HttpWebResponse)clientRequest.GetResponse();

            // String to hold API request response data
            string responseString = null;
            
            // Reads data to string from servers response and prints it to console
            using (Stream stream = serverResponse.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                responseString = sr.ReadToEnd();
                Console.WriteLine(responseString);
                sr.Close();
                serverResponse.Dispose();
            }

            // Finds & returns the Certificate from the Microsoft Certificate Store based on the thumbprint you pass to it
            // ... or returns null if it can't find a cert matching the thumbprint
            X509Certificate GetX509Certificate(string thumbprint)
            {
                var certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certificateStore.Open(OpenFlags.ReadOnly);

                var certs = certificateStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (certs.Count == 0)
                    return null;

                return certs.OfType<X509Certificate>().FirstOrDefault();
            }          
        }
    }
}

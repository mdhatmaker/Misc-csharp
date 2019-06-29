using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAPI
{
    public class DataObject
    {
        public string Name { get; set; }

    } // END OF CLASS

    public class WebClient
    {
        //private const string URL = "https://sub.domain.com/objects.json";
        //private string urlParameters = "?api_key=123";

        private string _url;

        public WebClient(string url)
        {
            _url = url;
        }

        public void Submit(string urlParameters)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_url);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;
                foreach (var d in dataObjects)
                {
                    Console.WriteLine("{0}", d.Name);
                }
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
        }

    } // END OF CLASS


} // END OF NAMESPACE

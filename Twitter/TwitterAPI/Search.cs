using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Credentials;

namespace TwitterAPI
{
    public class Search
    {
        /*
        // I'll modify this code to perform Twitter Search
        var appUrl = "http://api.twitter.com/1/statuses/update.xml?status=Hello";
        var authzHeader = oauth.GenerateAuthzHeader(appUrl, "POST");
        var request = (HttpWebRequest)WebRequest.Create(appUrl);
        request.Method = "POST";
        request.PreAuthenticate = true;
        request.AllowWriteStreamBuffering = true;
        request.Headers.Add("Authorization", authzHeader);

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            if (response.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("There's been a problem trying to tweet:" +
                                Environment.NewLine +
                                response.StatusDescription);
        }
        */

        public static void UpdateStatus(OAuthManager oauth, string status)
        {
            //var appUrl = "http://api.twitter.com/1/statuses/update.xml?status=Hello";
            var appUrl = "http://api.twitter.com/1/statuses/update.xml?status=" + status;

            appUrl = "https://api.twitter.com/1.1/statuses/update.json?status=Posting%20from%20%40apigee's%20API%20test%20console.%20It's%20like%20a%20command%20line%20for%20the%20Twitter%20API!%20%23apitools&display_coordinates=false";
            
            var authzHeader = oauth.GenerateAuthzHeader(appUrl, "POST");
            var request = (HttpWebRequest)WebRequest.Create(appUrl);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;
            request.Headers.Add("Authorization", authzHeader);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine("There's been a problem trying to tweet:" + Environment.NewLine + response.StatusDescription);
            }

        }

        public static void PerformSearch(string searchFor)
        {
            

        }


        public static void PerformSearch(OAuthManager oauth, string searchFor)
        {
            searchFor = "%40twitterapi";

            // now, update twitter status using that access token
            var appUrl = "https://api.twitter.com/1.1/search/tweets.json?q=" + searchFor;
            var authzHeader = oauth.GenerateAuthzHeader(appUrl, "POST");
            var request = (HttpWebRequest)WebRequest.Create(appUrl);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;
            request.Headers.Add("Authorization", authzHeader);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //MessageBox.Show("There's been a problem trying to tweet:" + Environment.NewLine + response.StatusDescription);
                    Console.WriteLine("There's been a problem trying to tweet:" + Environment.NewLine + response.StatusDescription);
                }
            }
        }

        
        public static void DoIt()
        {
            //const string URL = "https://sub.domain.com/objects.json";
            //const string urlParameters = "?api_key=123";
            const string URL = "https://api.twitter.com/1.1/search/tweets.json";
            const string urlParameters = "?q=%23freebandnames";
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;   // blocking call!!!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!!!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;
                foreach (var d in dataObjects)
                {
                    Console.WriteLine("{0}", d.Name);
                }
            }
        }

    } // END OF CLASS


} // END OF NAMESPACE

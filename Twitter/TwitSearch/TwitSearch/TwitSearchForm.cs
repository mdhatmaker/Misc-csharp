using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using TwitterAPI;

namespace TwitSearch
{
    public partial class TwitSearchForm : Form
    {
        private const string MY_APP_SPECIFIC_CONSUMER_KEY = "1NJMjL0Zb90nNTJ7qnctBAB0x";
        private const string MY_APP_SPECIFIC_CONSUMER_SECRET = "Wy8lprPaAvlJqpsK7nwjdLYJqIV6lGfFGWJ9vwhkybBnt0Ltd7";

        private const string URL_REQUEST_TOKEN = "https://api.twitter.com/oauth/request_token";     // the URL to obtain a temporary "request token"
        private const string SERVICE_SPECIFIC_AUTHORIZE_URL_STUB = "https://api.twitter.com/oauth/authorize?oauth_token=";
        private const string URL_ACCESS_TOKEN = "https://api.twitter.com/oauth/access_token";       // the URL to get the "access token"

        private WebClient _client;

        private OAuthManager _oauth = new OAuthManager();

        public TwitSearchForm()
        {
            InitializeComponent();

            //_client = new WebClient("https://sub.domain.com/objects.json");
            _client = new WebClient("https://api.twitter.com/1.1/search/tweets.json");
        }

        private void testSubmit()
        {
            //_client.Submit("?api_key=123");
            _client.Submit("?q=%40twitterapi");
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            _oauth["consumer_key"] = MY_APP_SPECIFIC_CONSUMER_KEY;
            _oauth["consumer_secret"] = MY_APP_SPECIFIC_CONSUMER_SECRET;
            _oauth.AcquireRequestToken(URL_REQUEST_TOKEN, "POST");

            var url = SERVICE_SPECIFIC_AUTHORIZE_URL_STUB + _oauth["token"];
            webBrowser1.Visible = true;
            webBrowser1.Url = new Uri(url);
            // FOR EXTERNAL BROWSER: System.Diagnostics.Process.Start(url);

            //testSubmit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TwitterAPI.Search.DoIt();
            //getPinAndSearch();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //var index = webBrowser1.DocumentText.LastIndexOf("and enter this PIN to complete");
            var index = webBrowser1.DocumentText.LastIndexOf("granted access to TragTracker");
            Console.WriteLine("webBrowser1_DocumentCompleted: index={0}", index);

            if (index >= 0)
            {
                //getPinAndSearch();
            }
        }

        private void getPinAndSearch()
        {
            var pin = getPIN();

            //webBrowser1.Visible = false;
            //webBrowser1.Dispose();

            _oauth.AcquireAccessToken(URL_ACCESS_TOKEN, "POST", pin);

            // Now you have access tokens, and you can use them in signed HTTP requests. Like this:
            // var authzHeader = oauth.GenerateAuthzHeader(url, "POST");
            // ...where url is the resource endpoint.
            // To update the user's status, it would be "http://api.twitter.com/1/statuses/update.xml?status=Hello".
            // Then set that string into the HTTP Header named Authorization.

            // To interact with third-party services, like TwitPic, you need to construct a slightly different OAuth header, like this:
            // var authzHeader = oauth.GenerateCredsHeader(URL_VERIFY_CREDS, "GET", AUTHENTICATION_REALM);
            // For Twitter, the values for the verify creds url and realm are "https://api.twitter.com/1/account/verify_credentials.json"
            // and "http://api.twitter.com/" respectively.
            // ...and put that authorization string in an HTTP header called X-Verify-Credentials-Authorization.
            // Then send that to your service, like TwitPic, along with whatever request you're sending.

            // That's it.

            Search.UpdateStatus(_oauth, "Hello");
            //Search.PerformSearch(_oauth, "#hatman");
        }

        private string getPIN()
        {
            var divMarker = "<div id=\"oauth_pin\">"; // the div for twitter's oauth pin
            var index = webBrowser1.DocumentText.LastIndexOf(divMarker) + divMarker.Length;
            var snip = webBrowser1.DocumentText.Substring(index);
            var pin = Regex.Replace(snip, "(?s)[^0-9]*([0-9]+).*", "$1").Trim();

            var elements = webBrowser1.Document.GetElementsByTagName("CODE");
            pin = elements[0].InnerText;

            return pin;
        }


    } // END OF CLASS
} // END OF NAMESPACE

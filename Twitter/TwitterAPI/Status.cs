using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAPI
{
    /*
    All together, the code to update twitter status might be something like this:
    
    // the URL to obtain a temporary "request token"
    var rtUrl = "https://api.twitter.com/oauth/request_token";
    var oauth = new OAuth.Manager();
    // The consumer_{key,secret} are obtained via registration
    oauth["consumer_key"] = "~~~CONSUMER_KEY~~~~";
    oauth["consumer_secret"] = "~~~CONSUMER_SECRET~~~";
    oauth.AcquireRequestToken(rtUrl, "POST");
    var authzUrl = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth["token"];
    // here, should use a WebBrowser control. 
    System.Diagnostics.Process.Start(authzUrl);  // example only!
    // instruct the user to type in the PIN from that browser window
    var pin = "...";
    var atUrl = "https://api.twitter.com/oauth/access_token";
    oauth.AcquireAccessToken(atUrl, "POST", pin);

    // now, update twitter status using that access token
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

    public class Status
    {

    } // END OF CLASS

} // END OF NAMESPACE

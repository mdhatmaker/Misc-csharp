using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Cloud.Storage.V1;


///  https://developers.google.com/youtube/v3/code_samples/dotnet


namespace Google.Apis.YouTube.Samples
{
    /// <summary>
    /// Based on YouTube Data API v3 sample: retrieve my uploads.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://developers.google.com/api-client-library/dotnet/get_started
    /// </summary>
    internal class YoutubePlaylists
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube Data API: Playlists");
            Console.WriteLine("===========================");

            var credential_filename = @"C:\Users\mhatm\OneDrive\Documents\MyCredentials\youtoober-service-account.json";

            // NOTE: Environment variable GOOGLE_APPLICATION_CREDENTIALS should point to your local (JSON) credentials file
            //set GOOGLE_APPLICATION_CREDENTIALS="C:\Users\mhatm\OneDrive\Documents\MyCredentials\youtoober-service-account.json"
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\mhatm\OneDrive\Documents\MyCredentials\youtoober-service-account.json")
            //var app_creds = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            //var rv = AuthImplicit("formal-archive-265821");
            //var user_creds = ReadClientSecretsJson(credential_filename, "TempFolder1");

            try
            {
                new YoutubePlaylists().Run(credential_filename).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // Test your implicit authentication credentials (GOOGLE_APPLICATION_CREDENTIALS environment variable)
        public static object AuthImplicit(string projectId)
        {
            // If you don't specify credentials when constructing the client, the
            // client library will look for credentials in the environment.
            var credential = GoogleCredential.GetApplicationDefault();

            //var ytube = YouTube.v3.YouTubeService.
            
            /*var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }*/
            return null;
        }

        public static async Task<UserCredential> ReadClientSecretsJson(string localFileName, string remoteFolderName)
        {
            UserCredential credential = null;
            //var localFileName = @"client_secrets.json";
            //string remoteFolderName = this.GetType().ToString();
            using (var stream = new FileStream(localFileName, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                    // user's account, but not other types of account access.
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None,                    
                    new FileDataStore(remoteFolderName)
                );
            }
            return credential;
        }


        private async Task Run(string credentialFilename)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialFilename, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    // unused - VisionService sample code
                    //.CreateScoped(VisionService.Scope.CloudPlatform);
                    .CreateScoped(YouTubeService.Scope.Youtube);
            }

            // unused - VisionService sample code
            /*var service = new VisionService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "my-app-name",
            });*/


            // Create our authenticated YouTubeService, which we'll use for API requests:
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            //---------------------------- FUNCTIONS ----------------------------------
            Func<string,int,IList<Playlist>> getPlaylists = (channelId,maxResults) =>
              {
                  var playlistListRequest = youtubeService.Playlists.List("snippet,contentDetails");
                  playlistListRequest.ChannelId = channelId;
                  playlistListRequest.MaxResults = maxResults;
                  //playlistListRequest.Mine = true;

                  // Retrieve the snippet and contentDetails parts of the Playlists resource for the given ChannelId.
                  var playlistListResponse = playlistListRequest.Execute();   //= await playlistListRequest.ExecuteAsync();

                  return playlistListResponse.Items;
              };

            Func<string,int,IList<PlaylistItem>> getPlaylistItems = (playlistId,maxResults) =>
              {
                  List<PlaylistItem> items = new List<PlaylistItem>();
                  string nextPageToken = "";
                  while (nextPageToken != null)
                  {
                      var playlistRequest = youtubeService.PlaylistItems.List("snippet,contentDetails");
                      playlistRequest.PlaylistId = playlistId;
                      playlistRequest.MaxResults = maxResults;
                      playlistRequest.PageToken = nextPageToken;

                      var playlistResponse = playlistRequest.Execute(); //= await playlistRequest.ExecuteAsync();

                      items.AddRange(playlistResponse.Items);
                      nextPageToken = playlistResponse.NextPageToken;
                  }
                  return items;
              };

            Func<IList<Playlist>, string, int> printPlaylists = (playlists, append) =>
              {
                  int nlists = 0;
                  foreach (var playlist in playlists)
                  {
                      Console.WriteLine("{0,3}) kind:{1}    id:{2}    ({3} items)", ++nlists, playlist.Kind, playlist.Id, playlist.ContentDetails.ItemCount);
                      var snip = playlist.Snippet;
                      Console.WriteLine("     {0}      [{1}] [{2}]         '{3}'", snip.PublishedAt, snip.ChannelId, snip.ChannelTitle, snip.Title);
                  }
                  Console.Write(append);
                  return nlists;
              };

            Func<IList<PlaylistItem>, string, int> printPlaylistItems = (playlistItems, append) =>
              {
                  int nitems = 0;
                  foreach (var item in playlistItems)
                  {
                      Console.WriteLine("{0}) {1}", ++nitems, item.Snippet.Title);
                  }
                  Console.Write(append);
                  return nitems;
              };
            //-------------------------------------------------------------------------

            // Find the playlists for the specified Channel (using ChannelId)
            string chanId = "UCvFrkS8V9jzKOqZdDdkm25Q";
            var plists = getPlaylists(chanId, 50);
            printPlaylists(plists, "\n\n\n");

            // Find the playlist with the specified Title and get its items:
            string playlistTitle = "Coding and Tech";
            Console.WriteLine(">>> Search for Playlist with Title: '{0}'", playlistTitle);
            var pl = plists.First(p => p.Snippet.Title == playlistTitle);

            //var watchlaters = getPlaylistItems("WL", 50);

            Console.WriteLine(">>> Getting Playlist with Id: '{0}'\n", pl.Id);
            var pitems = getPlaylistItems(pl.Id, 50);
            printPlaylistItems(pitems, "\n\n\n");



            //------------------- some scratch code --------------------
            /*///https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions
            // Func<T,TResult> delegate:
            Func<int, int> square = x => x * x;
            Console.WriteLine(square(5));   // Output: 25
            // Expression lambdas can be converted to the 'expression tree' types:
            System.Linq.Expressions.Expression<Func<int, int>> e = x => x * x;
            Console.WriteLine(e);   // Output: x => (x * x)
            // You can use lambda expressions when you write LINQ in C#:
            int[] numbers = { 2, 3, 4, 5 };
            var squaredNumbers = numbers.Select(x => x * x);
            Console.WriteLine(string.Join(" ", squaredNumbers));*/
            //----------------------------------------------------------


            //return;
            

            var channelsListRequest = youtubeService.Channels.List("contentDetails");
            channelsListRequest.ForUsername = "CurbsideExit";
            //channelsListRequest.Mine = true;

            // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            var channelsListResponse = await channelsListRequest.ExecuteAsync();

            foreach (var channel in channelsListResponse.Items)
            {
                // From the API response, extract the playlist ID that identifies the list
                // of videos uploaded to the authenticated user's channel.
                //var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
                var uploadsListId = channel.ContentDetails.RelatedPlaylists.WatchLater;

                Console.WriteLine("Videos in list {0}", uploadsListId);

                var pageToken = "";
                while (pageToken != null)
                {
                    var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                    playlistItemsListRequest.PlaylistId = uploadsListId;
                    playlistItemsListRequest.MaxResults = 50;
                    playlistItemsListRequest.PageToken = pageToken;

                    // Retrieve the list of videos uploaded to the authenticated user's channel.
                    var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

                    foreach (var playlistItem in playlistItemsListResponse.Items)
                    {
                        // Print information about each video.
                        Console.WriteLine("{0} ({1})", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
                    }

                    pageToken = playlistItemsListResponse.NextPageToken;
                }
            }
        }
    } // end of class

    // This is no longer needed, because List<T> (not IList<T>) already has an AddRange method
    /*public static class ExtensionMethods
    {
        // Extension method to allow appending one IList<T> to another IList<T>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var cur in enumerable)
            {
                collection.Add(cur);
            }
        }
    } // end of class ExtensionMethods*/

} // end of namespace
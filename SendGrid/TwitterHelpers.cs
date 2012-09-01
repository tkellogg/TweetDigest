using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TweetSharp;

namespace SendGrid
{
    public class TwitterHelpers
    {
        public IEnumerable<TwitterStatus> GetFavorites()
        {
            var service = new TwitterService(Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret);

            // Step 1 - Retrieve an OAuth Request Token
            var requestToken = service.GetRequestToken();

            // Step 2 - Redirect to the OAuth Authorization URL
            var uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());

            // Step 3 - Exchange the Request Token for an Access Token
            Console.Write("Please, your key? ");
            var verifier = Console.ReadLine(); // <-- This is input into your application by your user
            var access = service.GetAccessToken(requestToken, verifier);

            // Step 4 - User authenticates using the Access Token
            service.AuthenticateWith(access.Token, access.TokenSecret);

            var users = service.SearchForUser("kellogh");
            if (users == null)
            {
                Console.WriteLine("No users found");
                yield break;
            }

            var user = users.First();

            var favorites = service.ListFavoriteTweetsFor(user.Id);

            foreach (var tweet in favorites)
            {
                yield return tweet;
            }
        }
    }
}
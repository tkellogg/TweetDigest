using System.Collections.Generic;
using MongoDB.Bson;
using TweetSharp;

namespace Culminator.Models
{
    public class User
    {
        private TwitterAuthData authData;
        private List<string> twitterHandles;

        public User()
        {
            TwitterHandles = TwitterHandles ?? new List<string>();
            AuthData = new TwitterAuthData();
        }

        public BsonObjectId Id { get; set; }
        public string Email { get; set; }

        public TwitterAuthData AuthData
        {
            get { return authData; }
            set
            {
                if (value == null) return;
                authData = value;
            }
        }

        public List<string> TwitterHandles
        {
            get { return twitterHandles; }
            private set
            {
                if (value == null) return;
                twitterHandles = value;
            }
        }
    }

    public class TwitterAuthData
    {
        public string Token { get; set; }
        public string Secret { get; set; }
    }

    public class SetEmailViewModel
    {
        public string UserEmail { get; set; }
    }

    public class AuthenticatedHomePageViewModel
    {
        public string Email { get; set; }
        public string ProfilePicUrl { get; set; }
        public string TwitterHandle { get; set; }
    }
}
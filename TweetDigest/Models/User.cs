﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using TweetSharp;

namespace TweetDigest.Models
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
        public Guid? LoginKey { get; set; }

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

        public DateTime EpochOfTweets { get; set; }
    }

    public class TwitterAuthData
    {
        public string Token { get; set; }
        public string Secret { get; set; }
        public int? TwitterId { get; set; }

        public bool IsCompleted { get { return !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(Secret); } }
    }

    public class SetEmailViewModel
    {
        public string UserEmail { get; set; }
    }

    public class AuthenticatedHomePageViewModel
    {
        public AuthenticatedHomePageViewModel(User user, ITwitterFactory twitter)
        {
            Email = user.Email;
            var twitterUser = twitter.CurrentUser;
            if (twitterUser == null) throw new InvalidOperationException("twitterUser was not found or was just null");
            ProfilePicUrl = twitterUser.ProfileImageUrl;
            TwitterHandle = twitterUser.ScreenName;
            var service = twitter.Create();
            if (twitterUser == null) throw new InvalidOperationException("Twitter service was null or just couldn't be constructed");
            Favorites = service.ListFavoriteTweets(2, 5);
        }

        public string Email { get; private set; }
        public string ProfilePicUrl { get; private set; }
        public string TwitterHandle { get; private set; }
        public IEnumerable<TwitterStatus> Favorites { get; private set; } 
    }
}
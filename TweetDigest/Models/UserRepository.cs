﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace TweetDigest.Models
{
    public interface IUserRepository
    {
        void Save(User user);
        User GetById(BsonObjectId id);
        User GetByEmail(string email);
        User GetByLoginKey(Guid loginKey);
        IEnumerable<User> GetAll();
    }

    public class UserRepository : IUserRepository
    {
        private readonly MongoCollection<User> users;

        public UserRepository(MongoCollection<User> users)
        {
            this.users = users;
        }

        public void Save(User user)
        {
            users.Save(user);
        }

        public User GetById(BsonObjectId id)
        {
            return users.FindOneById(id);
        }

        public User GetByEmail(string email)
        {
            return (from user in users.AsQueryable()
                    where user.Email == email
                    select user).FirstOrDefault();
        }

        public User GetByLoginKey(Guid loginKey)
        {
            return (from user in users.AsQueryable()
                    where user.LoginKey == loginKey
                    select user).FirstOrDefault();
        }

        public IEnumerable<User> GetAll()
        {
            return users.AsQueryable();
        }
    }
}
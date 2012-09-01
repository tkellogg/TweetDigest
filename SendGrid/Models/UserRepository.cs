using System;
using MongoDB.Bson;
using MongoDB.Driver;
using TweetSharp;

namespace SendGrid.Models
{
    public interface IUserRepository
    {
        void Save(User user);
        User GetById(BsonObjectId id);
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
    }
}
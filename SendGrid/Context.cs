using System.Web;
using Culminator.Models;
using MongoDB.Bson;

namespace Culminator
{
    public interface IContext
    {
        BsonObjectId UserId { get; set; }
        User User { get; }
    }

    public class Context : IContext
    {
        private readonly IUserRepository userRepository;

        public Context(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public BsonObjectId UserId
        {
            get { return HttpContext.Current.Session["userId"] as BsonObjectId; }
            set { HttpContext.Current.Session["userId"] = value; }
        }

        public User User
        {
            get
            {
                var userId = UserId;
                if (userId == null) return null;
                return userRepository.GetById(userId);
            }
        }
    }
}
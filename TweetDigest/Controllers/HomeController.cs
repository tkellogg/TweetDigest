using System;
using System.Web.Mvc;
using MongoDB.Bson;
using TweetDigest.Models;
using TweetSharp;

namespace TweetDigest.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IContext context;
        private readonly ITwitterFactory twitterFactory;
        private readonly IMailController mailController;

        public HomeController(IUserRepository userRepository, IContext context, ITwitterFactory twitterFactory,
            IMailController mailController)
        {
            this.userRepository = userRepository;
            this.context = context;
            this.twitterFactory = twitterFactory;
            this.mailController = mailController;
        }

        /// <summary>
        /// 1. Landing page where they enter their Email
        /// </summary>
        public ActionResult Index()
        {
            var user = context.User;
            if (user == null) return View();

            return View("AuthenticatedHomePage", GetAuthenticatedHomePageViewModel(user));
        }

        /// <summary>
        /// 2. They submit their email address
        /// </summary>
        [HttpPost]
        public ActionResult Index(SetEmailViewModel model)
        {
            var user = userRepository.GetByEmail(model.UserEmail);
            if (user != null && user.AuthData.IsCompleted)
            {
                mailController.LoginEmail(user).Deliver();
                return View("PleaseCheckEmail");
            }

            user = user ?? new User {Email = model.UserEmail};
            userRepository.Save(user);
            context.UserId = user.Id;

            // 3. Then they are forwarded to Twitter to get authorization
            var uri = twitterFactory.GetAuthorizationUri();
            return RedirectPermanent(uri.ToString());
        }

        private AuthenticatedHomePageViewModel GetAuthenticatedHomePageViewModel(User user)
        {
            return new AuthenticatedHomePageViewModel(user, twitterFactory);
        }

        /// <summary>
        /// 4. Twitter redirects back to this action where we finish creating the user
        /// </summary>
        public ActionResult AuthorizeCallback(string oauth_token, string oauth_verifier)
        {
            var requestToken = new OAuthRequestToken {Token = oauth_token};
            var service = twitterFactory.Create();
            var accessToken = service.GetAccessToken(requestToken, oauth_verifier);

            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            var twitterUser = service.VerifyCredentials();
            if (twitterUser == null) return HttpNotFound("Dude, Twitter has no idea who you are.");

            var user = userRepository.GetByTwitterId(twitterUser.Id) ?? context.User;
            if (user == null) return HttpNotFound("I'm sorry, I don't remember seeing you before. Have we met?");

            user.AuthData = new TwitterAuthData 
                    {Secret = accessToken.TokenSecret, Token = accessToken.Token, TwitterId = twitterUser.Id};
            user.TwitterHandles.Add(twitterUser.ScreenName);
            userRepository.Save(user);

            return RedirectToAction("Index");
        }

        public ActionResult Login(Guid id)
        {
            var user = userRepository.GetByLoginKey(id);
            if (user == null) return View("AccessDenied");
            user.LoginKey = null;
            userRepository.Save(user);
            context.UserId = user.Id;
            
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            context.UserId = null;
            return RedirectToAction("Index");
        }

        public ActionResult Unsubscribe(string id)
        {
            var userId = BsonObjectId.Parse(id);
            userRepository.RemoveById(userId);
            context.UserId = null;
            return View("Unsubscribed");
        }
    }
}

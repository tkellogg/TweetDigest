using System;
using System.Web.Mvc;
using MongoDB.Bson;
using SendGrid.Models;
using TweetSharp;

namespace SendGrid.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;

        public HomeController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// 1. Landing page where they enter their Email
        /// </summary>
        public ActionResult Index()
        {
            var userId = Session["userId"] as BsonObjectId;
            if (userId == null) return View();

            var user = userRepository.GetById(userId);
            var viewModel = new AuthenticatedHomePageViewModel
                {
                    Email = user.Email
                };
            return View("AuthenticatedHomePage", viewModel);
        }

        /// <summary>
        /// 2. They submit their email address
        /// </summary>
        [HttpPost]
        public ActionResult Index(SetEmailViewModel model)
        {
            var user = new User {Email = model.UserEmail};
            userRepository.Save(user);
            Session["userId"] = user.Id;
            return Authorize();
        }

        /// <summary>
        /// 3. Then they are forwarded to Twitter to get authorization
        /// </summary>
        private ActionResult Authorize()
        {
            var service = new TwitterService(Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret);
            var requestToken = service.GetRequestToken();
            var uri = service.GetAuthorizationUri(requestToken);
            return RedirectPermanent(uri.ToString());
        }

        /// <summary>
        /// 4. Twitter redirects back to this action where we finish creating the user
        /// </summary>
        public ActionResult AuthorizeCallback(string oauth_token, string oauth_verifier)
        {
            var requestToken = new OAuthRequestToken {Token = oauth_token};
            var service = new TwitterService(Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret);
            var accessToken = service.GetAccessToken(requestToken, oauth_verifier);

            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            var twitterUser = service.GetUserProfile();
            var userId = (BsonObjectId) Session["userId"];
            var user = userRepository.GetById(userId);
            user.AuthData = new TwitterAuthData 
                    {Secret = accessToken.TokenSecret, Token = accessToken.Token};
            user.TwitterHandles.Add(twitterUser.ScreenName);
            userRepository.Save(user);
//            Session["userId"] = user.Id;

            // Auth
            // 1. Create a sessions collection - capped, expires after a month or two
            // 2. { sessionId, userId, startDate }
            // 3. Store sessionId in cookie, hashed with something other unique goodie
            // 
            // User flows
            // 1. Come to site, get involved by entering your email + twitter handle
            // 2. Forward to Twitter who has you authorize
            // 3. Forwarded back to /AuthorizeCallback which creates the account and sets session cookie
            // 4. Forwarded to home page that displays your latest favorites
            //
            // 1. Come back to site, session ended so click "send me an email"
            // 2. Go to email, click link to get back in

            return RedirectToAction("Index");
        }
    }
}

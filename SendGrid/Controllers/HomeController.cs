using System.Web.Mvc;
using Culminator.Models;
using MongoDB.Bson;
using TweetSharp;

namespace Culminator.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IContext context;
        private readonly ITwitterFactory twitterFactory;

        public HomeController(IUserRepository userRepository, IContext context, ITwitterFactory twitterFactory)
        {
            this.userRepository = userRepository;
            this.context = context;
            this.twitterFactory = twitterFactory;
        }

        /// <summary>
        /// 1. Landing page where they enter their Email
        /// </summary>
        public ActionResult Index()
        {
            var user = context.User;
            if (user == null) return View();

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
            context.UserId = user.Id;
            return Authorize();
        }

        /// <summary>
        /// 3. Then they are forwarded to Twitter to get authorization
        /// </summary>
        private ActionResult Authorize()
        {
            var service = twitterFactory.Create();
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
            var service = twitterFactory.Create();
            var accessToken = service.GetAccessToken(requestToken, oauth_verifier);

            service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
            var twitterUser = service.VerifyCredentials();
            var user = context.User;
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

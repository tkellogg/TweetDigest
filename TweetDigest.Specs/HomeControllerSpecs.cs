using System;
using System.Web.Mvc;
using MongoDB.Bson;
using Moq;
using Moq.AutoMock;
using Should;
using TweetDigest.Controllers;
using TweetDigest.Models;
using TweetSharp;
using Xunit;

namespace TweetDigest.Specs
{
    public class HomeControllerSpecs
    {
        public class When_getting_index
        {
            private readonly AutoMocker mocker = new AutoMocker();

            private HomeController Sut
            {
                get { return mocker.CreateInstance<HomeController>(); }
            }

            [Fact]
            public void It_forwards_to_default_when_no_user_is_authenticated()
            {
                var result = Sut.Index().ShouldBeType<ViewResult>();
                result.ViewName.ShouldBeEmpty();
            }

            [Fact]
            public void It_forwards_to_AuthenticatedHomePage_when_user_is_authenticated()
            {
                mocker.Setup<IContext>(x => x.User)
                    .Returns(new User {Email = "test@example.com"});
                mocker.Use<ITwitterFactory>(x => x.CurrentUser == new TwitterUser()
                                                 && x.Create() == Mock.Of<TwitterService>());

                var result = Sut.Index().ShouldBeType<ViewResult>();
                result.ViewName.ShouldEqual("AuthenticatedHomePage");
            }

            [Fact]
            public void It_sets_info_from_context_when_user_is_authenticated()
            {
                mocker.Setup<IContext>(x => x.User)
                    .Returns(new User {Email = "test@example.com"});
                mocker.Use<ITwitterFactory>(x => x.CurrentUser == new TwitterUser()
                                                 && x.Create() == Mock.Of<TwitterService>());

                var result = Sut.Index().ShouldBeType<ViewResult>();
                var model = result.Model.ShouldBeType<AuthenticatedHomePageViewModel>();
                model.Email.ShouldEqual("test@example.com");
            }
        }

        public class When_POSTing_to_Index
        {
            readonly AutoMocker mocker = new AutoMocker();
            HomeController Sut { get { return mocker.CreateInstance<HomeController>(); } }

            [Fact]
            public void It_sets_email__saves_and_redirects_to_twitter()
            {
                mocker.Setup<IUserRepository>(x => x.Save(It.IsAny<User>())).Callback<User>(u =>
                    {
                        u.Id = BsonObjectId.Empty;
                        u.Email.ShouldEqual("test@example.com");
                    });
                mocker.Setup<ITwitterFactory>(x => x.GetAuthorizationUri()).Returns(new Uri("http://twitter"));

                var viewModel = new SetEmailViewModel {UserEmail = "test@example.com"};
                Sut.Index(viewModel).ShouldBeType<RedirectResult>();

                mocker.VerifyAll();
                mocker.GetMock<IContext>().VerifySet(x => x.UserId);
            }

            [Fact]
            public void It_redirects_to_twitter_when_twitter_creds_arent_complete()
            {
                // user is NOT NULL but also invalid creds. Probably backed out of a signup before
                mocker.Setup<IUserRepository>(x => x.GetByEmail("test@example.com")).Returns(new User());
                mocker.Setup<ITwitterFactory>(x => x.GetAuthorizationUri()).Returns(new Uri("http://twitter"));

                var viewModel = new SetEmailViewModel {UserEmail = "test@example.com"};
                Sut.Index(viewModel).ShouldBeType<RedirectResult>();

                mocker.VerifyAll();
                mocker.GetMock<IContext>().VerifySet(x => x.UserId);
            }

            [Fact]
            public void It_sends_login_email_when_twitter_creds_are_already_stored()
            {
                var user = new User
                    {
                        AuthData = new TwitterAuthData {Secret = "foo", Token = "bar"}
                    };
                mocker.Use<IUserRepository>(x => x.GetByEmail("foo@bar.us") == user);

                Sut.Index(new SetEmailViewModel {UserEmail = "foo@bar.us"});
                mocker.Verify<IMailController>(x => x.LoginEmail(user));
            }
        }
    }
}

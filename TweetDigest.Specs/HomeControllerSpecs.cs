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
        readonly AutoMocker mocker = new AutoMocker();
        HomeController Sut { get { return mocker.CreateInstance<HomeController>(); } }

        [Fact]
        public void Index_forwards_to_default_when_no_user_is_authenticated()
        {
            var result = Sut.Index().ShouldBeType<ViewResult>();
            result.ViewName.ShouldBeEmpty();
        }

        [Fact]
        public void Index_forwards_to_AuthenticatedHomePage_when_user_is_authenticated()
        {
            mocker.Setup<IContext>(x => x.User)
                .Returns(new User {Email = "test@example.com"});

            var result = Sut.Index().ShouldBeType<ViewResult>();
            result.ViewName.ShouldEqual("AuthenticatedHomePage");
        }

        [Fact]
        public void Index_sets_info_from_context_when_user_is_authenticated()
        {
            mocker.Setup<IContext>(x => x.User)
                .Returns(new User {Email = "test@example.com"});

            var result = Sut.Index().ShouldBeType<ViewResult>();
            var model = result.Model.ShouldBeType<AuthenticatedHomePageViewModel>();
            model.Email.ShouldEqual("test@example.com");
        }

        [Fact]
        public void PostIndex_sets_email__saves_and_redirects_to_twitter()
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
        public void PostIndex_reuses_existing_user_and_sends_to_AuthenticatedHomePage_if_AuthData_is_valid()
        {
            var user = new User
                {
                    AuthData = new TwitterAuthData {Secret = "foo", Token = "bar"}
                };
            mocker.Setup<IUserRepository>(x => x.GetByEmail("test@example.com")).Returns(user);
            mocker.Setup<ITwitterFactory>(x => x.GetAuthorizationUri()).Returns(new Uri("http://twitter"));

            var viewModel = new SetEmailViewModel {UserEmail = "test@example.com"};
            var result = Sut.Index(viewModel).ShouldBeType<ViewResult>();

            result.ViewName.ShouldEqual("AuthenticatedHomePage");
            mocker.Verify<IUserRepository>(x => x.Save(It.IsAny<User>()), Times.Never());
            mocker.GetMock<IContext>().VerifySet(x => x.UserId, 
                "We need to set context.UserId because it was obviously not set in the previous HTTP request");
        }
    }
}

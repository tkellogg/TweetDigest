using System;
using System.Web.Mvc;
using Culminator.Controllers;
using Culminator.Models;
using MongoDB.Bson;
using Moq;
using Moq.AutoMock;
using Should;
using TweetSharp;
using Xunit;

namespace Culminator.Specs
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
        public void PostIndex_sets_email_and_saves()
        {
            mocker.Setup<IUserRepository>(x => x.Save(It.IsAny<User>())).Callback<User>(u =>
                {
                    u.Id = BsonObjectId.Empty;
                    u.Email.ShouldEqual("test@example.com");
                });
            var twitter = new Mock<TwitterService>() {CallBase = false};
            twitter.Setup(x => x.GetRequestToken()).Returns(new OAuthRequestToken());
            twitter.Setup(x => x.GetAuthorizationUri(It.IsAny<OAuthRequestToken>()))
                .Returns(new Uri("http://localhost"));
            mocker.Setup<ITwitterFactory>(x => x.Create()).Returns(twitter.Object);

            var viewModel = new SetEmailViewModel {UserEmail = "test@example.com"};
            Sut.Index(viewModel);

            mocker.VerifyAll();
            mocker.GetMock<IContext>().VerifySet(x => x.UserId);
        }
    }
}

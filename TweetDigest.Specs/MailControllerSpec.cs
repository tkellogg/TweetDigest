using System;
using Moq;
using Moq.AutoMock;
using Should;
using TweetDigest.Models;
using Xunit;

namespace TweetDigest.Specs
{
    public class MailControllerSpec
    {
        readonly AutoMocker mocker = new AutoMocker();
        MailController Sut { get { return mocker.CreateSelfMock<MailController>(); } }

        [Fact]
        public void It_sets_the_loginKey_for_the_user()
        {
            AssertUser(u => u.LoginKey.ShouldNotEqual(null));
            Sut.LoginEmail(new User{Email = "foo@bar.us"});
            mocker.VerifyAll();
        }

        [Fact]
        public void It_sends_the_correct_Guid_in_the_URL()
        {
            Guid? key = null;
            AssertUser(u => key = u.LoginKey);
            var sut = Sut;
            Mock.Get(sut).Setup(x => x.Email("LoginEmail", It.IsAny<object>(), null, true))
                .Callback<string, object, string, bool>((a, model, b, c) => 
                    model.ShouldBeType<LoginEmailViewModel>()
                    .ReturnUrl.ShouldContain(key.ToString()));

            sut.LoginEmail(new User{Email = "foo@bar.us"});
            mocker.VerifyAll();
        }

        void AssertUser(Action<User> assert)
        {
            mocker.Setup<IUserRepository>(u => u.Save(It.IsAny<User>())).Callback(assert);
        }
    }
}

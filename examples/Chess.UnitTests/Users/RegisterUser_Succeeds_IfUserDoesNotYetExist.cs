using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Users
{
    [TestClass]
    public sealed class RegisterUser_Succeeds_IfUserDoesNotYetExist : UnitTest<RegisterUserCommand>
    {
        public readonly Guid UserId;
        public readonly string UserName;

        public RegisterUser_Succeeds_IfUserDoesNotYetExist() :
            this("John") { }

        public RegisterUser_Succeeds_IfUserDoesNotYetExist(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }
            UserId = Guid.NewGuid();
            UserName = userName;
        }        

        protected override RegisterUserCommand WhenMessageIsHandled() =>
            new RegisterUserCommand(UserId, UserName);

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Result.IsEventStreamAsync(1, stream =>
            {
                AssertEvent<UserRegisteredEvent>(stream, 0, @event =>
                {
                    Assert.AreEqual(UserId, @event.UserId);
                    Assert.AreEqual(1, @event.UserVersion);
                    Assert.AreEqual(UserName, @event.UserName);
                });
            });      
        }              
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Users
{
    [TestClass]
    public sealed class RegisterUser_Fails_IfUserAlreadyExists : UnitTest<RegisterUserCommand>
    {
        private readonly RegisterUser_Succeeds_IfUserDoesNotYetExist _userIsRegistered;
        private readonly Guid _userId;

        public RegisterUser_Fails_IfUserAlreadyExists()
        {
            _userIsRegistered = new RegisterUser_Succeeds_IfUserDoesNotYetExist();
            _userId = Guid.NewGuid();
        }

        protected override IEnumerable<IMessageStream> Given()
        {
            yield return _userIsRegistered;
        }

        protected override RegisterUserCommand WhenMessageIsHandled() =>
            new RegisterUserCommand(_userId, _userIsRegistered.UserName);

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Result.IsExceptionOfTypeAsync<UnprocessableEntityException>(exception =>
            {
                AssertInnerExceptionIsOfType<IllegalOperationException>(exception, innerException =>
                {
                    Assert.AreEqual("", innerException.Message);
                });
            });            
        }        
    }
}

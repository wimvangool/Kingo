using System;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Users
{
    [TestClass]
    public sealed class RegisterUserCommandTest : RequestMessageTest
    {
        #region [====== Invalid UserId ======]

        private const string _UserId = nameof(RegisterUserCommand.UserId);

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfUserIdIsEmpty()
        {
            AssertIsNotValid(new RegisterUserCommand(Guid.Empty, NewUserName()), 1)
                .AssertMemberError(_UserId);            
        }

        #endregion

        #region [====== Invalid UserName ======]

        private const string _UserName = nameof(RegisterUserCommand.UserName);

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfUserNameIsNull()
        {
            AssertIsNotValid(new RegisterUserCommand(Guid.NewGuid(), null), 1)
                .AssertMemberError(_UserName);            
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfUserNameIsNoIdentifier()
        {
            AssertIsNotValid(new RegisterUserCommand(Guid.NewGuid(), "%$"), 1)
                .AssertMemberError(_UserName);            
        }                

        #endregion

        #region [====== Valid Messages ======]

        [TestMethod]
        public void Validate_ReturnsNoError_IfMessageIsValid()
        {
            AssertIsValid(new RegisterUserCommand(Guid.NewGuid(), NewUserName()));           
        }

        private static string NewUserName() =>
            "John_" + Clock.Current.UtcDateAndTime().Millisecond;

        #endregion
    }
}

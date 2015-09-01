using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
{
    /// <summary>
    /// Tests the behavior of <see cref="RegisterPlayerCommand" /> instances.
    /// </summary>
    [TestClass]
    public sealed class RegisterPlayerCommandTest : MessageTest<RegisterPlayerCommand>
    {
        #region [====== Message Factory Methods ======]

        private const string _ValidUsername = "John";
        private const string _ValidPassword = "Doe_1234567";

        protected override RegisterPlayerCommand CreateValidMessage()
        {
            return new RegisterPlayerCommand(_ValidUsername, _ValidPassword);
        }

        protected override RegisterPlayerCommand Change(RegisterPlayerCommand message)
        {
            var username = message.Username + "#";
            var password = message.Password + "#";

            return new RegisterPlayerCommand(username, password);
        }

        #endregion

        #region [====== Validation - Username ======]

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameIsNull()
        {
            var message = new RegisterPlayerCommand(null, _ValidPassword);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameIsEmpty()
        {
            var message = new RegisterPlayerCommand(string.Empty, _ValidPassword);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameContainsOnlyWhiteSpace()
        {
            var message = new RegisterPlayerCommand("    ", _ValidPassword);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameContainsAnIllegalCharacter()
        {
            var message = new RegisterPlayerCommand("abcd+3", _ValidPassword);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameIsTooShort()
        {
            var message = new RegisterPlayerCommand("abc", _ValidPassword);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameIsTooLong()
        {
            var message = new RegisterPlayerCommand("abcdefghijklm", _ValidPassword);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        #endregion

        #region [====== Validation - Password ======]

        [TestMethod]
        public void Validate_ReturnsErrors_IfPasswordIsNull()
        {
            var message = new RegisterPlayerCommand(_ValidUsername, null);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Password"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfPasswordIsEmpty()
        {
            var message = new RegisterPlayerCommand(_ValidUsername, string.Empty);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Password"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfPasswordContainsOnlyWhiteSpace()
        {
            var message = new RegisterPlayerCommand(_ValidUsername, "       ");
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Password"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfPasswordContainsAnIllegalCharacter()
        {
            var message = new RegisterPlayerCommand(_ValidUsername, "abcd 3g5g65");
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Password"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfPasswordIsTooShort()
        {
            var message = new RegisterPlayerCommand(_ValidUsername, "abcde");
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Password"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfPasswordIsTooLong()
        {
            var message = new RegisterPlayerCommand(_ValidUsername, "abcdefghijklmnopqrstuvw");
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Password"]);
        }

        #endregion
    }
}
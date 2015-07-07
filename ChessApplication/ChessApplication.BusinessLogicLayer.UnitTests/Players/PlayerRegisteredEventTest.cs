using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syztem;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// Tests the behavior of <see cref="PlayerRegisteredEvent" /> instances.
    /// </summary>
    [TestClass]
    public sealed class PlayerRegisteredEventTest : MessageTest<PlayerRegisteredEvent>
    {
        #region [====== Message Factory Methods ======]

        private static readonly Guid _ValidPlayerId = Guid.NewGuid();
        private static readonly DateTimeOffset _ValidPlayerVersion = Clock.Current.LocalDateAndTime();
        private const string _ValidUsername = "Peter";

        protected override PlayerRegisteredEvent CreateValidMessage()
        {
            return new PlayerRegisteredEvent(_ValidPlayerId, _ValidPlayerVersion)
            {
                Username = _ValidUsername
            };
        }

        protected override PlayerRegisteredEvent Change(PlayerRegisteredEvent message)
        {
            var messageCopy = message.Copy();

            messageCopy.Username = message.Username + "#";

            return messageCopy;
        }

        #endregion

        #region [====== Validate - Id ======]

        [TestMethod]
        public void Validate_ReturnsErrors_IfIdentifierIsEmpty()
        {
            var message = new PlayerRegisteredEvent(Guid.Empty, _ValidPlayerVersion)
            {
                Username = _ValidUsername
            };
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["PlayerId"]);
        }

        #endregion

        #region [====== Validate - Username ======]

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameIsNull()
        {
            var message = new PlayerRegisteredEvent(_ValidPlayerId, _ValidPlayerVersion);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameIsEmpty()
        {
            var message = new PlayerRegisteredEvent(_ValidPlayerId, _ValidPlayerVersion)
            {
                Username = string.Empty
            };
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        [TestMethod]
        public void Validate_ReturnsErrors_IfUsernameContainsOnlyWhiteSpace()
        {
            var message = new PlayerRegisteredEvent(_ValidPlayerId, _ValidPlayerVersion)
            {
                Username = "    "
            };
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsNotNull(errorTree.Errors["Username"]);
        }

        #endregion
    }
}
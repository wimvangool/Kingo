using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games
{
    [TestClass]
    public sealed class MovePieceCommandTest
    {
        #region [====== From ======]

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfFromIsNull()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), null, GenerateRandomPosition());

            message.Validate().AssertMemberError("'From' is not specified.", "From");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfFromIsLessThanTwo()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), "a", GenerateRandomPosition());

            message.Validate().AssertMemberError("'a' is not a valid position identifier.", "From");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfFromIsGreaterThanTwo()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), "abc", GenerateRandomPosition());

            message.Validate().AssertMemberError("'abc' is not a valid position identifier.", "From");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfFirstCharacterOfFromIsNotAColumn()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), "11", GenerateRandomPosition());

            message.Validate().AssertMemberError("'11' is not a valid position identifier.", "From");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfSecondCharacterOfFromIsNotARow()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), "a0", GenerateRandomPosition());

            message.Validate().AssertMemberError("'a0' is not a valid position identifier.", "From");
        }

        #endregion

        #region [====== To ======]

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfToIsNull()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), null);

            message.Validate().AssertMemberError("'To' is not specified.", "To");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfToIsLessThanTwo()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "a");

            message.Validate().AssertMemberError("'a' is not a valid position identifier.", "To");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfToIsGreaterThanTwo()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "abc");

            message.Validate().AssertMemberError("'abc' is not a valid position identifier.", "To");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfFirstCharacterOfToIsNotAColumn()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "11");

            message.Validate().AssertMemberError("'11' is not a valid position identifier.", "To");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfSecondCharacterOfToIsNotARow()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "a0");

            message.Validate().AssertMemberError("'a0' is not a valid position identifier.", "To");
        }

        #endregion

        #region [====== Valid Messages ======]

        private static readonly char[] _FilePositions = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private static readonly Random _RandomNumberGenerator = new Random();

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {
            var message = new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), GenerateRandomPosition());

            message.Validate().AssertNoErrors();
        }

        private static string GenerateRandomPosition()
        {
            var file = _FilePositions[NextBoardPosition()];
            var rank = NextBoardPosition() + 1;

            return $"{file}{rank}";
        }

        private static int NextBoardPosition()
        {
            lock (_RandomNumberGenerator)
            {
                return _RandomNumberGenerator.Next(0, 8);
            }
        }

        #endregion
    }
}

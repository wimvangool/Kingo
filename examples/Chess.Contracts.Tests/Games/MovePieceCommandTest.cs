using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games
{
    [TestClass]
    public sealed class MovePieceCommandTest : RequestMessageTest
    {
        #region [====== Invalid From ======]

        private const string _From = nameof(MovePieceCommand.From);

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfFromIsNull()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), null, GenerateRandomPosition()), 1)
                .AssertMemberError(_From);           
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfFromIsLessThanTwo()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), "a", GenerateRandomPosition()), 1)
                .AssertMemberError(_From);            
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfFromIsGreaterThanTwo()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), "abc", GenerateRandomPosition()), 1)
                .AssertMemberError(_From);            
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfFirstCharacterOfFromIsNotAColumn()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), "11", GenerateRandomPosition()), 1)
                .AssertMemberError(_From);           
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfSecondCharacterOfFromIsNotARow()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), "a0", GenerateRandomPosition()), 1)
                .AssertMemberError(_From);            
        }

        #endregion

        #region [====== Invalid To ======]

        private const string _To = nameof(MovePieceCommand.To);

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfToIsNull()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), null), 1)
                .AssertMemberError(_To);            
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfToIsLessThanTwo()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "a"), 1)
                .AssertMemberError(_To);            
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfLengthOfToIsGreaterThanTwo()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "abc"), 1)
                .AssertMemberError(_To);            
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfFirstCharacterOfToIsNotAColumn()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "11"), 1)
                .AssertMemberError(_To);            
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfSecondCharacterOfToIsNotARow()
        {
            AssertIsNotValid(new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), "a0"), 1)
                .AssertMemberError(_To);            
        }

        #endregion

        #region [====== Valid Messages ======]

        private static readonly char[] _FilePositions = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private static readonly Random _RandomNumberGenerator = new Random();

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {
            AssertIsValid(new MovePieceCommand(Guid.NewGuid(), GenerateRandomPosition(), GenerateRandomPosition()));            
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// Tests the behavior of <see cref="ChallengePlayerCommand" /> instances.
    /// </summary>
    [TestClass]
    public sealed class ChallengePlayerCommandTest : MessageTest<ChallengePlayerCommand>
    {
        #region [====== Message Factory Methods ======]

        protected override ChallengePlayerCommand CreateValidMessage()
        {
            return new ChallengePlayerCommand(Guid.NewGuid(), Guid.NewGuid());
        }

        protected override ChallengePlayerCommand CreateUnequalCopyOf(ChallengePlayerCommand message)
        {
            return CreateValidMessage();
        }

        #endregion

        #region [====== Validate - SenderId ======]

        // TODO...

        #endregion
    }
}
using System;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Domain;

namespace Kingo.ChessApplication.Challenges
{
    /// <summary>
    /// Handles the <see cref="RejectChallengeCommand" />.
    /// </summary>	
    [MessageHandler(InstanceLifetime.PerResolve)]
    public sealed class RejectChallengeCommandHandler : MessageHandler<RejectChallengeCommand>
    {
        private readonly IChallengeRepository _challenges;

        /// <summary>
        /// Initializes a new instance of the <see cref="RejectChallengeCommandHandler" /> class.
        /// </summary>
        /// <param name="challenges">A <see cref="IChallengeRepository" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="challenges" /> is <c>null</c>.
        /// </exception>
        public RejectChallengeCommandHandler(IChallengeRepository challenges)
        {
            if (challenges == null)
            {
                throw new ArgumentNullException("challenges");
            }
            _challenges = challenges;
        }

        /// <inheritdoc />
        [Throws(typeof(AggregateNotFoundByKeyException<Guid>))]
        [Throws(typeof(ChallengeAlreadyAcceptedException), ConvertToCommandExecutionException = true)]
        [Throws(typeof(ChallengeAlreadyRejectedException), ConvertToCommandExecutionException = true)]
        public override async Task HandleAsync(RejectChallengeCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);

            challenge.Reject();
        }
    }
}
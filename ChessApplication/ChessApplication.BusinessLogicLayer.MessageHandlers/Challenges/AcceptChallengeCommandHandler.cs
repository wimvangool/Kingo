using System;
using System.Threading.Tasks;
using Syztem.ComponentModel.Server;
using Syztem.ComponentModel.Server.Domain;

namespace SummerBreeze.ChessApplication.Challenges
{
    /// <summary>
    /// Handles the <see cref="AcceptChallengeCommand" />.
    /// </summary>	
    [MessageHandler(InstanceLifetime.PerResolve)]
    public sealed class AcceptChallengeCommandHandler : MessageHandler<AcceptChallengeCommand>
    {
        private readonly IChallengeRepository _challenges;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptChallengeCommandHandler" /> class.
        /// </summary>
        /// <param name="challenges">A <see cref="IChallengeRepository" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="challenges" /> is <c>null</c>.
        /// </exception>
        public AcceptChallengeCommandHandler(IChallengeRepository challenges)
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
        public override async Task HandleAsync(AcceptChallengeCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);

            challenge.Accept();
        }
    }
}
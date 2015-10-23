﻿using System;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Domain;
using Kingo.ChessApplication.Challenges;

namespace Kingo.ChessApplication.Players
{
    /// <summary>
    /// Handles the <see cref="ChallengePlayerCommand" />.
    /// </summary>	
    [MessageHandler(InstanceLifetime.PerResolve)]
    public sealed class ChallengePlayerCommandHandler : MessageHandler<ChallengePlayerCommand>
    {
        private readonly IPlayerRepository _players;
        private readonly IChallengeRepository _challenges;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengePlayerCommandHandler" /> class.
        /// </summary>
        /// <param name="players">A <see cref="IPlayerRepository" />.</param>
        /// <param name="challenges">A <see cref="IChallengeRepository" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="players" /> or <paramref name="challenges" />is <c>null</c>.
        /// </exception>
        public ChallengePlayerCommandHandler(IPlayerRepository players, IChallengeRepository challenges)
        {
            if (players == null)
            {
                throw new ArgumentNullException("players");
            }
            if (challenges == null)
            {
                throw new ArgumentNullException("challenges");
            }
            _players = players;
            _challenges = challenges;
        }

        /// <inheritdoc />
        [Throws(typeof(AggregateNotFoundByKeyException<Guid>))]
        public override async Task HandleAsync(ChallengePlayerCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var getSenderTask = _players.GetByIdAsync(message.SenderId);
            var getReceiverTask = _players.GetByIdAsync(message.ReceiverId);

            var sender = await getSenderTask;
            var receiver = await getReceiverTask;

            _challenges.Add(sender.ChallengeOtherPlayer(message.ChallengeId, receiver));
        }
    }
}
using System;
using System.ComponentModel.Server;
using System.Threading.Tasks;
using SummerBreeze.ChessApplication.Challenges;

namespace SummerBreeze.ChessApplication.Games
{
    /// <summary>
    /// Handles the <see cref="ChallengeAcceptedEvent" />.
    /// </summary>	
    [MessageHandler(InstanceLifetime.PerResolve)]
    public sealed class GameStarter : MessageHandler<ChallengeAcceptedEvent>
    {
        private readonly IChallengeRepository _challenges;
        private readonly IGameRepository _games;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStarter" /> class.
        /// </summary>
        /// <param name="challenges">A <see cref="IChallengeRepository" />.</param>
        /// <param name="games">A <see cref="IGameRepository" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="challenges" /> or <paramref name="games"/> is <c>null</c>.
        /// </exception>
        public GameStarter(IChallengeRepository challenges, IGameRepository games)
        {
            if (challenges == null)
            {
                throw new ArgumentNullException("challenges");
            }
            if (games == null)
            {
                throw new ArgumentNullException("games");
            }
            _challenges = challenges;
            _games = games;
        }

        /// <inheritdoc />        
        public override async Task HandleAsync(ChallengeAcceptedEvent message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var challenge = await _challenges.GetByIdAsync(message.ChallengeId);
            var newGame = challenge.StartNewGame();

            _games.Add(newGame);
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Players
{
    /// <summary>
    /// Represents a repository of players.
    /// </summary>
    public interface IPlayerRepository
    {
        /// <summary>
        /// Indicates whether or not a player with a certain name has been registered.
        /// </summary>
        /// <param name="playerName">Name of the player to check.</param>
        /// <returns><c>true</c> if the name was registered; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="playerName"/> is <c>null</c>.
        /// </exception>
        Task<bool> HasBeenRegisteredAsync(Identifier playerName);

        /// <summary>
        /// Adds a player to the repository.
        /// </summary>
        /// <param name="player">The player to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="player"/> is <c>null</c>.
        /// </exception>
        void Add(Player player);
    }
}

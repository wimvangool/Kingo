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
        /// Returns the player with the specified name.
        /// </summary>
        /// <param name="playerId">Name of the player.</param>
        /// <returns>The player with the specified name.</returns>        
        Task<Player> GetByIdAsync(Guid playerId);        

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

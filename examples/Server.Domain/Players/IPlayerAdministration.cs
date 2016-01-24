using System;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Players
{
    /// <summary>
    /// When implemented by a class, represents an administration of players that can answer several
    /// questions about registered players.
    /// </summary>
    public interface IPlayerAdministration
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
    }
}

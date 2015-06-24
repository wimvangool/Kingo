using System;
using System.ComponentModel.Server.Domain;
using System.Threading.Tasks;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// Manages the persistence of <see cref="Player" /> instances.
    /// </summary>
    public interface IPlayerRepository
    {
        /// <summary>
        /// Retrieves a player by its key.
        /// </summary>
        /// <param name="id">Unique identifier of the player.</param>
        /// <returns>A task that will retrieve the player.</returns>
        Task<Player> GetById(Guid id);

        /// <summary>
        /// Adds a player to the repository.
        /// </summary>
        /// <param name="player">The player to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="player" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="DuplicateKeyException{T, S}">
        /// <paramref name="player" />'s id matches the id of another aggregate that
        /// was already added to this repository.
        /// </exception>
        void Add(Player player);
    }
}
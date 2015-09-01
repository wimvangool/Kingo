using System;
using System.Threading.Tasks;

namespace Kingo.ChessApplication.Games
{
    /// <summary>
    /// Manages the persistence of <see cref="Game" /> instances.
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Retrieves a game by its key.
        /// </summary>
        /// <param name="id">Unique identifier of the game.</param>
        /// <returns>A task that will retrieve the game.</returns>
        Task<Game> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a game to the repository.
        /// </summary>
        /// <param name="game">The game to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="game" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="DuplicateKeyException{TAggregate,TKey}">
        /// <paramref name="game" />'s id matches the id of another aggregate that
        /// was already added to this repository.
        /// </exception>
        void Add(Game game);
    }
}
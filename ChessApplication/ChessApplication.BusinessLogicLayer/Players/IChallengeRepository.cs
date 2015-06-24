using System;
using System.ComponentModel.Server.Domain;
using System.Threading.Tasks;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// Manages the persistence of <see cref="Challenge" /> instances.
    /// </summary>
    public interface IChallengeRepository
    {
        /// <summary>
        /// Retrieves a challenge by its key.
        /// </summary>
        /// <param name="id">Unique identifier of the challenge.</param>
        /// <returns>A task that will retrieve the challenge.</returns>
        Task<Challenge> GetById(Guid id);

        /// <summary>
        /// Adds a challenge to the repository.
        /// </summary>
        /// <param name="challenge">The challenge to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="challenge" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="DuplicateKeyException{T, S}">
        /// <paramref name="challenge" />'s id matches the id of another aggregate that
        /// was already added to this repository.
        /// </exception>
        void Add(Challenge challenge);
    }
}
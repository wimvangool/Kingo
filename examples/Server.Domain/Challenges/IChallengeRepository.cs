using System;

namespace Kingo.Samples.Chess.Challenges
{
    /// <summary>
    /// Represents a repository of challenges.
    /// </summary>
    public interface IChallengeRepository
    {
        /// <summary>
        /// Adds a challenge to the repository.
        /// </summary>
        /// <param name="challenge">The challenge to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="challenge"/> is <c>null</c>.
        /// </exception>
        void Add(Challenge challenge);
    }
}

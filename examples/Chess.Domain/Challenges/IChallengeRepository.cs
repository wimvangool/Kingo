using System;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Challenges
{
    /// <summary>
    /// Represents a repository of challenges.
    /// </summary>
    public interface IChallengeRepository
    {        
        Task<Challenge> GetByIdAsync(Guid challengeId);
       
        Task AddAsync(Challenge challenge);
    }
}

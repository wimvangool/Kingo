using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class MemoryChallengeRepository : MemoryRepository<Guid, int, Challenge>, IChallengeRepository
    {
        Task<Challenge> IChallengeRepository.GetByIdAsync(Guid challengeId)
        {
            return GetByKeyAsync(challengeId);
        }        

        void IChallengeRepository.Add(Challenge challenge)
        {
            Add(challenge);
        }
    }
}

using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class MemoryChallengeRepository : MemoryRepository<Guid, int, Challenge>, IChallengeRepository
    {
        void IChallengeRepository.Add(Challenge challenge)
        {
            Add(challenge);
        }
    }
}

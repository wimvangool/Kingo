using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class ChallengeRepository : IChallengeRepository
    {
        private readonly MemoryRepository<Guid, Challenge> _repository;

        public ChallengeRepository()
        {
            _repository = new MemoryRepository<Guid, Challenge>(MemoryRepositoryBehavior.StoreSnapshots);
        }

        public Task<Challenge> GetByIdAsync(Guid challengeId) =>
            _repository.GetByIdAsync(challengeId);

        public Task AddAsync(Challenge challenge) =>
            _repository.AddAsync(challenge);
    }
}

using System;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class SqlChallengeRepository : SnapshotRepository<Challenge>, IChallengeRepository
    {
        private readonly ITypeToContractMap _map;

        public SqlChallengeRepository(ITypeToContractMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            _map = map;
        }

        protected override ITypeToContractMap TypeToContractMap
        {
            get { return _map; }
        }

        Task<Challenge> IChallengeRepository.GetByIdAsync(Guid challengeId)
        {
            return GetByKeyAsync(challengeId);
        }

        Task IChallengeRepository.AddAsync(Challenge challenge)
        {
            Add(challenge);
        }        
    }
}

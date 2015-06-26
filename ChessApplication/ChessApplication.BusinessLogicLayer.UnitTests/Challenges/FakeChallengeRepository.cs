using System;
using System.ComponentModel.Server;
using System.ComponentModel.Server.Domain;
using System.Threading.Tasks;

namespace SummerBreeze.ChessApplication.Challenges
{
    /// <summary>
    /// Represents an in-memory repository for Challenge instances.
    /// </summary>
    [MessageHandlerDependency(InstanceLifetime.PerUnitOfWork)]
    public sealed class FakeChallengeRepository : FakeRepository<Challenge, Guid, DateTimeOffset>, IChallengeRepository
    {
        Task<Challenge> IChallengeRepository.GetByIdAsync(Guid id)
        {
            return GetByKeyAsync(id);
        }

        void IChallengeRepository.Add(Challenge challenge)
        {
            Add(challenge);
        }        
    }
}
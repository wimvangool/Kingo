using System;
using System.Threading.Tasks;
using ServiceComponents.ComponentModel.Server;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Challenges
{
    /// <summary>
    /// Represents an in-memory repository for Challenge instances.
    /// </summary>
    [MessageHandlerDependency(InstanceLifetime.PerUnitOfWork)]
    public sealed class FakeChallengeRepository : IChallengeRepository
    {
        private readonly FakeRepository<Challenge, Guid, DateTimeOffset> _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeChallengeRepository" /> class.
        /// </summary>
        public FakeChallengeRepository()
        {
            _implementation = new FakeRepository<Challenge, Guid, DateTimeOffset>();
        }

        /// <inheritdoc />
        public Task<Challenge> GetByIdAsync(Guid id)
        {
            return _implementation.GetByKeyAsync(id);
        }

        /// <inheritdoc />
        public void Add(Challenge challenge)
        {
            _implementation.Add(challenge);
        }        
    }
}
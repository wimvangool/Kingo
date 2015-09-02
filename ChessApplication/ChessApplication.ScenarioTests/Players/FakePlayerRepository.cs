using System;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Domain;

namespace Kingo.ChessApplication.Players
{
    /// <summary>
    /// Represents an in-memory repository for Player instances.
    /// </summary>
    [MessageHandlerDependency(InstanceLifetime.PerUnitOfWork)]
    public sealed class FakePlayerRepository : IPlayerRepository
    {
        private readonly FakeRepository<Player, Guid, DateTimeOffset> _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakePlayerRepository" /> class.
        /// </summary>
        public FakePlayerRepository()
        {
            _implementation = new FakeRepository<Player, Guid, DateTimeOffset>();
        }

        /// <inheritdoc />
        public Task<Player> GetByIdAsync(Guid id)
        {
            return _implementation.GetByKeyAsync(id);
        }

        /// <inheritdoc />
        public void Add(Player player)
        {
            _implementation.Add(player);
        }        
    }
}
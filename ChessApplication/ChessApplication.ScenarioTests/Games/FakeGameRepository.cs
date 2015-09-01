using System;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.ComponentModel.Server.Domain;

namespace Kingo.ChessApplication.Games
{
    /// <summary>
    /// Represents an in-memory repository for Game instances.
    /// </summary>
    [MessageHandlerDependency(InstanceLifetime.PerUnitOfWork)]
    public sealed class FakeGameRepository : IGameRepository
    {
        private readonly FakeRepository<Game, Guid, DateTimeOffset> _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeGameRepository" /> class.
        /// </summary>
        public FakeGameRepository()
        {
            _implementation = new FakeRepository<Game, Guid, DateTimeOffset>();
        }

        /// <inheritdoc />
        public Task<Game> GetByIdAsync(Guid id)
        {
            return _implementation.GetByKeyAsync(id);
        }

        /// <inheritdoc />
        public void Add(Game game)
        {
            _implementation.Add(game);
        }
    }
}
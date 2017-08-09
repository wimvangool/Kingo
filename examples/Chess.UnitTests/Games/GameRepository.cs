using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    public sealed class GameRepository : IGameRepository
    {
        private readonly MemoryRepository<Guid, Game> _repository;

        public GameRepository()
        {
            _repository = new MemoryRepository<Guid, Game>(MemoryRepositoryBehavior.StoreEvents);
        }

        public Task<Game> GetByKeyAsync(Guid gameId) =>
            _repository.GetByIdAsync(gameId);

        public Task AddAsync(Game game) =>
            _repository.AddAsync(game);
    }
}

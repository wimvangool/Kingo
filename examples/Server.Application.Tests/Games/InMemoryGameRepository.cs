using System;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    public sealed class InMemoryGameRepository : InMemoryRepository<Guid, int, Game>, IGameRepository
    {
        Task<Game> IGameRepository.GetByKeyAsync(Guid gameId)
        {
            return GetByKeyAsync(gameId);
        }

        void IGameRepository.Add(Game game)
        {
            Add(game);
        }        
    }
}

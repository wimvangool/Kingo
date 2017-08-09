using System;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Games
{
    public interface IGameRepository
    {
        Task<Game> GetByKeyAsync(Guid gameId);

        Task AddAsync(Game game);
    }
}

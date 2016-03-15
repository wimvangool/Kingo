using System;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Games
{
    public interface IGameRepository
    {
        Task<Game> GetByKeyAsync(Guid gameId);

        void Add(Game game);
    }
}

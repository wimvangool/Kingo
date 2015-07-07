using System;
using System.Threading.Tasks;
using Syztem.ComponentModel.Server;
using Syztem.ComponentModel.Server.Domain;

namespace SummerBreeze.ChessApplication.Games
{
    /// <summary>
    /// Represents an in-memory repository for Game instances.
    /// </summary>
    [MessageHandlerDependency(InstanceLifetime.PerUnitOfWork)]
    public sealed class FakeGameRepository : FakeRepository<Game, Guid, DateTimeOffset>, IGameRepository
    {
        Task<Game> IGameRepository.GetByIdAsync(Guid id)
        {
            return GetByKeyAsync(id);
        }

        void IGameRepository.Add(Game game)
        {
            Add(game);
        }
    }
}
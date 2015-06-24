using System;
using System.Collections.Generic;
using System.ComponentModel.Server;
using System.ComponentModel.Server.Domain;
using System.Threading.Tasks;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// Represents an in-memory repository for Player instances.
    /// </summary>
    [MessageHandlerDependency(InstanceLifetime.PerUnitOfWork)]
    public sealed class FakePlayerRepository : FakeRepository<Player, Guid, DateTimeOffset>, IPlayerRepository
    {
        Task<Player> IPlayerRepository.GetById(Guid id)
        {
            return GetByKey(id);
        }

        void IPlayerRepository.Add(Player player)
        {
            Add(player);
        }        
    }
}
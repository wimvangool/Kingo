using System;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Players;
using Kingo.Threading;

namespace Kingo.Samples.Chess.RegisterPlayer
{
    public sealed class PlayerRepository : MemoryRepository<Guid, int, Player>, IPlayerRepository
    {
        Task<bool> IPlayerRepository.HasBeenRegisteredAsync(Identifier playerName)
        {
            return AsyncMethod.RunSynchronously(() => Aggregates.Values.Any(aggregate => aggregate.Name == playerName));
        }

        void IPlayerRepository.Add(Player player)
        {
            Add(player);
        }       
    }
}

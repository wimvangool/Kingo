using System;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Threading;

namespace Kingo.Samples.Chess.Players
{
    public sealed class InMemoryPlayerRepository : InMemoryRepository<Guid, int, Player>, IPlayerRepository, IPlayerAdministration
    {
        Task<Player> IPlayerRepository.GetByIdAsync(Guid playerId)
        {
            return GetByKeyAsync(playerId);
        }        

        Task<bool> IPlayerAdministration.HasBeenRegisteredAsync(Identifier playerName)
        {
            return AsyncMethod.RunSynchronously(() => HasBeenRegistered(playerName));
        }

        private bool HasBeenRegistered(Identifier playerName)
        {
            if (playerName == null)
            {
                throw new ArgumentNullException(nameof(playerName));
            }
            return Snapshots.Values.Any(player => IsMatch((Player) player.RestoreAggregate(), playerName));
        }

        void IPlayerRepository.Add(Player player)
        {
            Add(player);
        }        

        private static bool IsMatch(Player player, Identifier playerName)
        {
            return string.Compare(player.Name, playerName, StringComparison.OrdinalIgnoreCase) == 0;
        }        
    }
}

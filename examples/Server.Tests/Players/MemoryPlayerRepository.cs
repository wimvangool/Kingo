using System;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Threading;

namespace Kingo.Samples.Chess.Players
{
    public sealed class MemoryPlayerRepository : MemoryRepository<Guid, int, Player>, IPlayerRepository
    {
        Task<Player> IPlayerRepository.GetByIdAsync(Guid playerId)
        {
            return GetByKeyAsync(playerId);
        }        

        Task<bool> IPlayerRepository.HasBeenRegisteredAsync(Identifier playerName)
        {
            return AsyncMethod.RunSynchronously(() => HasBeenRegistered(playerName));
        }

        private bool HasBeenRegistered(Identifier playerName)
        {
            if (playerName == null)
            {
                throw new ArgumentNullException("playerName");
            }
            return Aggregates.Values.Any(player => IsMatch(player, playerName));
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

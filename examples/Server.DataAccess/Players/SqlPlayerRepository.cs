using System;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Players
{
    public sealed class SqlPlayerRepository : SnapshotRepository<Player>, IPlayerRepository
    {
        private readonly ITypeToContractMap _map;

        public SqlPlayerRepository(ITypeToContractMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }
            _map = map;
        }

        protected override ITypeToContractMap TypeToContractMap
        {
            get { return _map; }
        }

        Task<Player> IPlayerRepository.GetByIdAsync(Guid playerId)
        {
            return GetByKeyAsync(playerId);
        }       

        void IPlayerRepository.Add(Player player)
        {
            Add(player);
        }                                
    }
}

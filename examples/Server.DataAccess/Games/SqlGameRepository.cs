using System;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    public sealed class SqlGameRepository : EventStore<Game>, IGameRepository
    {
        private readonly ITypeToContractMap _map;

        public SqlGameRepository(ITypeToContractMap map)
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

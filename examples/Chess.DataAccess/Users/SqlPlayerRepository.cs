using System;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Users
{
    public sealed class SqlPlayerRepository : SnapshotRepository<User>, IUserRepository
    {
        private readonly ITypeToContractMap _map;

        public SqlPlayerRepository(ITypeToContractMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            _map = map;
        }

        protected override ITypeToContractMap TypeToContractMap
        {
            get { return _map; }
        }

        Task<User> IUserRepository.GetByIdAsync(Guid userId)
        {
            return GetByKeyAsync(userId);
        }       

        Task IUserRepository.AddAsync(User user)
        {
            AddAsync(user);
        }                                
    }
}

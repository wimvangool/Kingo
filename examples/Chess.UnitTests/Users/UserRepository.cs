using System;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Messaging.Validation;
using Kingo.Threading;

namespace Kingo.Samples.Chess.Users
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly MemoryRepository<Guid, User> _repository;

        public UserRepository()
        {
            _repository = new MemoryRepository<Guid, User>(MemoryRepositoryBehavior.StoreSnapshots);
        }

        public Task<bool> HasBeenRegisteredAsync(Identifier userName)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                var usersWithSpecifiedName =
                    from user in _repository
                    where user.Name.Equals(userName)
                    select user;

                return usersWithSpecifiedName.Any();
            });
        }

        public Task<User> GetByIdAsync(Guid userId) =>
            _repository.GetByIdAsync(userId);

        public Task AddAsync(User user) =>
            _repository.AddAsync(user);        
    }
}

using System;
using System.Threading.Tasks;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Users
{    
    public interface IUserRepository
    {
        Task<bool> HasBeenRegisteredAsync(Identifier userName);

        Task<User> GetByIdAsync(Guid userId);
        
        Task AddAsync(User user);
    }
}

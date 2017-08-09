using System.Threading.Tasks;
using Kingo.Samples.Chess.Challenges;

namespace Kingo.Samples.Chess.Users
{    
    public sealed class PlayerService : ServiceProcessor, IUserService
    {        
        #region [====== Write Methods ======]

        /// <inheritdoc />
        public Task RegisterUserAsync(RegisterUserCommand command)
        {
            return HandleAsync(command);
        }

        public Task ChallengePlayerAsync(ChallengeUserCommand command)
        {
            return HandleAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        /// <inheritdoc />
        public Task<GetRegisteredUsersResponse> GetPlayersAsync(GetPlayersRequest request)
        {
            return ExecuteAsync(UserTable.SelectAllAsync, request);
        }       

        #endregion
    }
}

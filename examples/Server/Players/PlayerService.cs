﻿using System.Threading.Tasks;
using Kingo.Samples.Chess.Challenges;

namespace Kingo.Samples.Chess.Players
{
    public sealed class PlayerService : WcfServiceProcessor, IPlayerService
    {
        #region [====== Write Methods ======]

        /// <inheritdoc />
        public Task RegisterPlayerAsync(RegisterPlayerCommand command)
        {
            return HandleAsync(command);
        }

        public Task ChallengePlayerAsync(ChallengePlayerCommand command)
        {
            return HandleAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        /// <inheritdoc />
        public Task<GetPlayersResponse> GetPlayersAsync(GetPlayersRequest request)
        {
            return ExecuteAsync(request, new GetPlayersQuery());
        }       

        #endregion
    }
}
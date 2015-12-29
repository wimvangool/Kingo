using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Challenges
{
    [ServiceContract]
    public interface IChallengeService
    {
        #region [====== Write Methods ======]

        /// <summary>
        /// Sends a challenge from one player to another.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A <see cref="Task" /> representing the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>        
        [OperationContract]
        Task ChallengePlayerAsync(ChallengePlayerCommand command);

        #endregion

        #region [====== Read Methods ======]

        /// <summary>
        /// Returns all received and pending challenges for the plyer that has been logged in.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <returns>A <see cref="Task{T}" /> representing the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        [OperationContract]
        Task<GetPendingChallengesResponse> GetPendingChallenges(GetPendingChallengesRequest request);

        #endregion
    }
}

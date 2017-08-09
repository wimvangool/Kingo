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
        Task ChallengePlayerAsync(ChallengeUserCommand command);

        /// <summary>
        /// Accepts the specified challenge.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A <see cref="Task" /> representing the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>    
        [OperationContract]
        Task AcceptChallengeAsync(AcceptChallengeCommand command);

        /// <summary>
        /// Rejects the specified challenge.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A <see cref="Task" /> representing the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>    
        [OperationContract]
        Task RejectChallengeAsync(RejectChallengeCommand command);

        #endregion

        #region [====== Read Methods ======]

        /// <summary>
        /// Returns all received and pending challenges for the plyer that has been logged in.
        /// </summary>        
        /// <returns>A <see cref="Task{T}" /> representing the operation.</returns>        
        [OperationContract]
        Task<GetPendingChallengesResponse> GetPendingChallenges();

        #endregion
    }
}

using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Players
{
    /// <summary>
    /// Exposes all features related to players.
    /// </summary>
    [ServiceContract]
    public interface IPlayerService
    {
        #region [====== Write Methods ======]

        /// <summary>
        /// Registers a new player in the system.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A <see cref="Task" /> representing the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>        
        [OperationContract]        
        Task RegisterPlayerAsync(RegisterPlayerCommand command);        

        #endregion

        #region [====== Read Methods ======]

        /// <summary>
        /// Returns all registered players.
        /// </summary>
        /// <param name="request">Request to execute.</param>
        /// <returns>A <see cref="Task{T}" /> representing the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>  
        [OperationContract]
        Task<GetPlayersResponse> GetPlayersAsync(GetPlayersRequest request);        

        #endregion
    }
}

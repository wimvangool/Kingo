using System;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Challenges
{
    /// <summary>
    /// This exception is thrown when a challenge is accepted or rejected when it has already been accepted.
    /// </summary>
    [Serializable]
    public sealed class ChallengeAlreadyAcceptedException : BusinessRuleViolationException
    {        
        private readonly Guid _challengeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeAlreadyAcceptedException" /> class.
        /// </summary>		
        /// <param name="challengeId">identifier of a challenge.</param>
        internal ChallengeAlreadyAcceptedException(Guid challengeId)
        {
            _challengeId = challengeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeAlreadyAcceptedException" /> class.
        /// </summary>	
        /// <param name="challengeId">identifier of a challenge.</param>	
        /// <param name="message">Message of the exception.</param>
        internal ChallengeAlreadyAcceptedException(Guid challengeId, string message)
            : base(message)
        {
            _challengeId = challengeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeAlreadyAcceptedException" /> class.
        /// </summary>	
        /// <param name="challengeId">identifier of a challenge.</param>	
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        internal ChallengeAlreadyAcceptedException(Guid challengeId, string message, Exception innerException)
            : base(message, innerException)
        {
            _challengeId = challengeId;
        }
        
        /// <summary>
        /// Identifier of a challenge.
        /// </summary>
        public Guid ChallengeId
        {
            get { return _challengeId; ;}
        }
    }
}
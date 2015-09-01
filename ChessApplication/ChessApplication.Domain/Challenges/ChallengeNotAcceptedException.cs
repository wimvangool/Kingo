using System;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Challenges
{
    /// <summary>
    /// This exception is thrown when an attempt is made to start a new game from a challenge that has
    /// not (yet) been accepted.
    /// </summary>
    [Serializable]
    public sealed class ChallengeNotAcceptedException : BusinessRuleViolationException
    {
        private readonly Guid _challengeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeNotAcceptedException" /> class.
        /// </summary>	
        /// <param name="challengeId">Identifier of a challenge.</param>	
        internal ChallengeNotAcceptedException(Guid challengeId)
        {
            _challengeId = challengeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeNotAcceptedException" /> class.
        /// </summary>
        /// <param name="challengeId">Identifier of a challenge.</param>		
        /// <param name="message">Message of the exception.</param>
        internal ChallengeNotAcceptedException(Guid challengeId, string message)
            : base(message)
        {
            _challengeId = challengeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeNotAcceptedException" /> class.
        /// </summary>		
        /// <param name="challengeId">Identifier of a challenge.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        internal ChallengeNotAcceptedException(Guid challengeId, string message, Exception innerException)
            : base(message, innerException)
        {
            _challengeId = challengeId;
        }    
    
        /// <summary>
        /// Identifier of a challenge.
        /// </summary>
        public Guid ChallengeId
        {
            get { return _challengeId; }
        }
    }
}
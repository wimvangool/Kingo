namespace SummerBreeze.ChessApplication.Challenges
{
    /// <summary>
    /// Represents the state a <see cref="Challenge" /> can be in.
    /// </summary>
    public enum ChallengeState
    {
        /// <summary>
        /// Indicates a new challenge that has yet to be accepted or rejected.
        /// </summary>
        New,

        /// <summary>
        /// Represents an accepted challenge.
        /// </summary>
        Accepted,

        /// <summary>
        /// Represents a rejected challenge.
        /// </summary>
        Rejected
    }
}

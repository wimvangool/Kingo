namespace YellowFlare.MessageProcessing.Server
{
    /// <summary>
    /// When implemented by a class, represents a scenario where a system is brought into a desired state by executing
    /// a specific sequence of messages.
    /// </summary>
    public interface IScenario : IMessageSequence
    {        
        /// <summary>
        /// Indicates that verification of the expected state as a result of this scenario has failed.
        /// </summary>
        void Fail();

        /// <summary>
        /// Indicates that verification of the expected state as a result of this scenario has failed.
        /// </summary>
        /// <param name="message">A message indicating which verification failed and why.</param>
        /// <param name="parameters">An optional array of parameters to include in the message.</param>
        void Fail(string message, params object[] parameters);        
    }
}

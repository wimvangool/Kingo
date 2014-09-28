namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// When implemented by a class, represents a scenario where a system is brought into a desired state by executing
    /// a specific sequence of messages.
    /// </summary>
    public interface IScenario : IMessageSequence
    {        
        /// <summary>
        /// Marks this scenario as failed.
        /// </summary>
        void Fail();

        /// <summary>
        /// Marks this scenario as failed using the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The reason why the scenario failed.</param>
        void Fail(string message);

        /// <summary>
        /// Marks this scenario as failed using the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The reason why the scenario failed.</param>
        /// <param name="parameters">An optional array of parameters to include in the message.</param>
        void Fail(string message, params object[] parameters);        
    }
}

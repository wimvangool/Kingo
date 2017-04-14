using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>           
    public abstract class Scenario<TResult> : Scenario where TResult : IScenarioResult
    {
        private readonly Lazy<TResult> _result;               

        internal Scenario()
        {
            _result = new Lazy<TResult>(CreateResult);                      
        }

        /// <summary>
        /// Returns the instance that is used verify the results of this scenario.
        /// </summary>
        public TResult Result =>
            _result.Value;

        internal abstract TResult CreateResult();               
    }
}

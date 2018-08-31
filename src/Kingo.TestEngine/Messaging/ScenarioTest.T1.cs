using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>           
    public abstract class ScenarioTest<TResult> : ScenarioTest where TResult : ITestResult
    {        
        /// <summary>
        /// Returns the result of this scenario that can be verified in a test.
        /// </summary>
        /// <returns></returns>
        protected abstract TResult ThenResult();               
    }
}

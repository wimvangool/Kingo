using System.ComponentModel.Messaging.Resources;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a <see cref="IConjunctionStatement{TValue}" /> that always fails.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public sealed class NullStatement<TValue> : IConjunctionStatement<TValue>
    {
        private readonly IScenario _scenario;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullStatement{TValue}" /> class.
        /// </summary>
        /// <param name="scenario">The scenario this statement belongs to.</param>
        public NullStatement(IScenario scenario)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException("scenario");
            }
            _scenario = scenario;
        }

        /// <summary>
        /// Failes the scenario.
        /// </summary>
        /// <param name="statement">Ignored.</param>
        public void And(Action<TValue> statement)
        {
            _scenario.Fail(FailureMessages.NullStatement_PreconditionFailed);
        }
    }
}

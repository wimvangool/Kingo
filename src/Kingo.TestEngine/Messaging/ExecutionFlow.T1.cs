using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Kingo.Messaging.Validation;
using Kingo.Messaging.Validation.Constraints;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a certain type of flow that can be defines for a scenario.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>
    public abstract class ExecutionFlow<TMessage> : ErrorMessageReader, IExecutable where TMessage : class, IMessage
    {
        private readonly MemberConstraintSet<Scenario<TMessage>> _validator;

        internal ExecutionFlow()
        {
            _validator = new MemberConstraintSet<Scenario<TMessage>>(true);
        }

        /// <inheritdoc /> 
        protected override IFormatProvider FormatProvider
        {
            get { return Scenario.FormatProvider; }
        }

        internal abstract Scenario<TMessage> Scenario
        {
            get;
        }

        internal IMemberConstraintSet<Scenario<TMessage>> Validator
        {
            get { return _validator; }
        }

        /// <inheritdoc /> 
        public void Execute()
        {
            ExecuteAsync().Wait();
        }

        /// <inheritdoc /> 
        public abstract Task ExecuteAsync();

        internal void ValidateExpectations()
        {
            _validator.WriteErrorMessages(Scenario, this);
        }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Add(string errorMessage, string memberName, ErrorInheritanceLevel inheritanceLevel)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }
            Scenario.OnVerificationFailed(errorMessage);
        }         
    }
}

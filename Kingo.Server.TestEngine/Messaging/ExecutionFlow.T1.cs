using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Kingo.Constraints;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a certain type of flow that can be defines for a scenario.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>
    public abstract class ExecutionFlow<TMessage> : ErrorMessageReader, IExecutable where TMessage : class, IMessage<TMessage>
    {
        private readonly MemberConstraintSet<Scenario<TMessage>> _validator;

        internal ExecutionFlow()
        {
            _validator = new MemberConstraintSet<Scenario<TMessage>>(true);
        }

        protected override IFormatProvider FormatProvider
        {
            get { return Scenario.FormatProvider; }
        }

        protected abstract Scenario<TMessage> Scenario
        {
            get;
        }

        protected IMemberConstraintSet<Scenario<TMessage>> Validator
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

        protected void ValidateExpectations()
        {
            _validator.WriteErrorMessages(Scenario, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Add(string errorMessage, string memberName, ErrorInheritanceLevel inheritanceLevel)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
            Scenario.OnVerificationFailed(errorMessage);
        } 

        protected static void Rethrow(Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}

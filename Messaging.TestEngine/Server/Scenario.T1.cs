using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is executed on the When-phase.</typeparam>    
    public abstract class Scenario<TMessage> : Scenario where TMessage : class, IMessage<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{T}" /> class.
        /// </summary>
        protected Scenario() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scenario{T}" /> class.
        /// </summary>   
        /// <param name="exceptionExpected">
        /// Indiciates whether or not a exception is expected to be thrown when this scenario is executed.
        /// </param>     
        protected Scenario(bool exceptionExpected)
        {            
            ExceptionExpected = exceptionExpected;            
        }                           

        /// <summary>
        /// Returns the last message that was handled in the When-phase.
        /// </summary>
        public TMessage Message
        {
            get;
            private set;
        }        

        /// <summary>
        /// Returns a sequence of messages that are used to put the system into a desired state.
        /// </summary>
        /// <returns>A sequence of messages that are used to put the system into a desired state.</returns>
        /// <remarks>
        /// The default implementation returns an empty sequence. When overridden, this method should never return
        /// <c>null</c>.
        /// </remarks>
        protected virtual IMessageSequence Given()
        {
            return EmptySequence;
        }                             

        /// <summary>
        /// Executes the scenario in two phases: first the <i>Given</i>-phase, followed by the <i>When</i>-phase.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Given-phase is represented by the set of messages that are returned by the <see cref="Given()" />-method.
        /// All returned messages are assumed to succeed. Any exceptions thrown during the Given-phase will not be
        /// caught and always result in a failed scenario test.
        /// </para>
        /// <para>
        /// The When-phase is represented by a single message, of which all effects are subject to test and verification.
        /// If an exception is expected to be thrown, then the <see cerf="ExceptionException" /> must be set to
        /// <c>true</c> before this method is called. If so, any exception will be caught and the <see cref="Exception" />
        /// property will be set accordingly. On the other hand, if <see cerf="ExceptionException" /> is set to <c>true</c>
        /// and no exception is thrown as a result of the message to test, then the scenario's <see cref="IScenario.Fail(string)" />
        /// method will be called with a message indicating the expected exception was not thrown. If no exception is
        /// expected, any exception is simply rethrown, assuming the scenario will fail automatically as a result.
        /// </para>
        /// <para>
        /// During the When-phase, all events that are published on the <see cref="MessageProcessorBus" /> are stored in a
        /// collection, ready to be verified.
        /// </para>        
        /// </remarks>
        public override void ProcessWith(IMessageProcessor processor)
        {            
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            Given().ProcessWith(processor);

            // This scenario must collect all events that are published during the When()-phase.
            using (processor.DomainEventBus.ConnectThreadLocal<object>(SaveDomainEvent, true))
            using (var scope = new UnitOfWorkScope(processor.DomainEventBus))
            {
                Message = When();

                HandleMessage(processor, Message);

                scope.Complete();
            }            
        }                       

        internal virtual void HandleMessage(IMessageProcessor processor, TMessage message)
        {
            try
            {
                processor.Handle(message); 
            }
            catch (Exception exception)
            {
                Exception = exception;

                if (!ExceptionExpected)
                {
                    throw;
                }
            }                        
        }        

        /// <summary>
        /// Returns a message that is used to test a system's behavior.
        /// </summary>
        /// <returns>A single message of which the effects will be verified in the Then-phase.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716", MessageId = "When", Justification = "'When' is part of the BDD-style naming convention.")]
        protected abstract TMessage When();                       
    }
}

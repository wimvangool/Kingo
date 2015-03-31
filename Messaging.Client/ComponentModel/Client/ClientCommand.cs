namespace System.ComponentModel.Client
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="INotifyIsExecuting" /> interface.
    /// </summary>  
    public class ClientCommand : ClientCommand<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>             
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public ClientCommand(IRequestDispatcher dispatcher)
            : base(dispatcher) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="isValidIndicator">
        /// The indicator that is used to determine whether the command can be executed.
        /// </param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public ClientCommand(IRequestDispatcher dispatcher, INotifyIsValid isValidIndicator)
            : base(dispatcher, isValidIndicator) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="isValidIndicator">
        /// The indicator that is used to determine whether the command can be executed.
        /// </param>
        /// <param name="options">
        /// The opions that determine the exact behavior of this command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public ClientCommand(IRequestDispatcher dispatcher, INotifyIsValid isValidIndicator, ClientCommandOptions options)
            : base(dispatcher, isValidIndicator, options) { }

        /// <inheritdoc />
        protected override bool TryConvertParameter(object parameterIn, out object parameterOut)
        {
            parameterOut = parameterIn;
            return true;
        }

        #region [====== Command Factory Methods - CommandDispatcher ======]

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand<TParameter> CreateCommand<TMessage, TParameter>(CommandDispatcher<TMessage> dispatcher) where TMessage : RequestMessageViewModel<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand<TParameter>(dispatcher, dispatcher.Message);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand<TParameter> CreateCommand<TMessage, TParameter>(CommandDispatcher<TMessage> dispatcher, ClientCommandOptions options) where TMessage : RequestMessageViewModel<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand<TParameter>(dispatcher, dispatcher.Message, options);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>        
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand CreateCommand<TMessage>(CommandDispatcher<TMessage> dispatcher, ClientCommandOptions options) where TMessage : RequestMessageViewModel<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand(dispatcher, dispatcher.Message, options);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is sent by the dispatcher.</typeparam>        
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand CreateCommand<TMessage>(CommandDispatcher<TMessage> dispatcher) where TMessage : RequestMessageViewModel<TMessage>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand(dispatcher, dispatcher.Message);
        }

        #endregion

        #region [====== Command Factory Methods - QueryDispatcher ======]

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand<TParameter> CreateCommand<TMessageIn, TMessageOut, TParameter>(QueryDispatcher<TMessageIn, TMessageOut> dispatcher)
            where TMessageIn : RequestMessageViewModel<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand<TParameter>(dispatcher, dispatcher.Message);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is sent by the dispatcher.</typeparam>
        /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
        /// <typeparam name="TParameter">Type of the parameter of the command.</typeparam>
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand<TParameter> CreateCommand<TMessageIn, TMessageOut, TParameter>(QueryDispatcher<TMessageIn, TMessageOut> dispatcher, ClientCommandOptions options)
            where TMessageIn : RequestMessageViewModel<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand<TParameter>(dispatcher, dispatcher.Message, options);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is sent by the dispatcher.</typeparam>    
        /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>    
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>        
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand CreateCommand<TMessageIn, TMessageOut>(QueryDispatcher<TMessageIn, TMessageOut> dispatcher)
            where TMessageIn : RequestMessageViewModel<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand(dispatcher, dispatcher.Message);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ClientCommand{T}" /> that encapsulates the specified <paramref name="dispatcher"/>.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is sent by the dispatcher.</typeparam>      
        /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>  
        /// <param name="dispatcher">The dispatcher that is used to execute all requests.</param>
        /// <param name="options">The opions that determine the exact behavior of this command.</param>
        /// <returns>A new <see cref="ClientCommand{T}" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public static ClientCommand CreateCommand<TMessageIn, TMessageOut>(QueryDispatcher<TMessageIn, TMessageOut> dispatcher, ClientCommandOptions options)
            where TMessageIn : RequestMessageViewModel<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            return new ClientCommand(dispatcher, dispatcher.Message, options);
        }

        #endregion
    }
}

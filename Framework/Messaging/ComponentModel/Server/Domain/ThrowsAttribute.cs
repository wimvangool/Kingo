namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This attribute can be declared on a <see cref="IMessageHandler{T}" /> class or on the
    /// <see cref="IMessageHandler{T}.HandleAsync" /> method of a <see cref="IMessageHandler{T}">
    /// MessageHandler</see> in order to catch any <see cref="DomainException" /> that can be
    /// thrown by the handler (or any delegated members) and convert it into a
    /// <see cref="InvalidMessageException" /> (the default) or a <see cref="CommandExecutionException" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ThrowsAttribute : Attribute, IDomainExceptionFilter
    {
        private readonly Type _exceptionType;
        private readonly bool _allowDerivedTypes;    

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowsAttribute" /> class.
        /// </summary>
        /// <param name="exceptionType">Type of exception to catch.</param>   
        /// <param name="allowDerivedTypes">
        /// Indicates whether or not the exception types must match exactly or that any derived types are also handled.
        /// </param>                 
        public ThrowsAttribute(Type exceptionType, bool allowDerivedTypes = true)
        {            
            _exceptionType = exceptionType;
            _allowDerivedTypes = allowDerivedTypes;
        }        

        /// <summary>
        /// Type of exception to catch.
        /// </summary>
        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        /// <summary>
        /// Indicates whether or not the caught exception of type <see cref="ExceptionType" />
        /// should be converted to a <see cref="CommandExecutionException" /> instead of a 
        /// <see cref="InvalidMessageException" />.
        /// </summary>
        public bool ConvertToCommandExecutionException
        {
            get;
            set;
        }

        /// <inheritdoc />
        public bool TryConvertToFunctionalException(IMessage message, DomainException domainException, out FunctionalException functionalException)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (domainException == null)
            {
                throw new ArgumentNullException("domainException");
            }
            if (IsMatch(domainException))
            {
                functionalException = ConvertToFunctionalException(message, domainException);
                return true;
            }
            functionalException = null;
            return false;
        }

        private bool IsMatch(DomainException domainException)
        {
            if (_exceptionType == null)
            {
                return false;
            }
            if (_allowDerivedTypes)
            {
                return _exceptionType.IsInstanceOfType(domainException);
            }
            return _exceptionType == domainException.GetType();
        }

        private FunctionalException ConvertToFunctionalException(IMessage message, DomainException domainException)
        {
            if (ConvertToCommandExecutionException)
            {
                return domainException.AsCommandExecutionException(message);
            }
            return domainException.AsInvalidMessageException(message);
        }        
    }
}

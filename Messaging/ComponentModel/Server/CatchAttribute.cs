using System.ComponentModel.Server.Domain;
using System.Resources;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// This attribute can be applied to the <see cref="IMessageHandler{T}.Handle">Handle</see> method
    /// of a <see cref="IMessageHandler{T}">MessageHandler</see> in order to catch any
    /// <see cref="ConstraintViolationException" /> that might be thrown by a <see cref="Repository{TAggregate,TKey,TVersion}" />
    /// while it is flushed and convert it into an <see cref="CommandExecutionException" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class CatchAttribute : Attribute
    {
        private readonly Type _exceptionType;        

        /// <summary>
        /// Initializes a new instance of the <see cref="CatchAttribute" /> class.
        /// </summary>
        /// <param name="exceptionType">Type of exception to catch.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exceptionType" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="exceptionType"/> is not assignable to a <see cref="ConstraintViolationException" />.
        /// </exception>
        public CatchAttribute(Type exceptionType)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException("exceptionType");
            }        
            if (typeof(ConstraintViolationException).IsAssignableFrom(exceptionType))
            {
                _exceptionType = exceptionType;
                return;
            }
            throw NewInvalidExceptionTypeException(exceptionType);
        }        

        /// <summary>
        /// Type of exception to catch.
        /// </summary>
        public Type ExceptionType
        {
            get { return _exceptionType; }
        }

        private static Exception NewInvalidExceptionTypeException(Type exceptionType)
        {
            var messageFormat = ExceptionMessages.CatchAttribute_InvalidExceptionType;
            var message = string.Format(messageFormat, exceptionType, typeof(ConstraintViolationException));
            return new ArgumentException(message, "exceptionType");
        }
    }
}

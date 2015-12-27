using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Constraints
{
    /// <summary>
    /// This exception is thrown when an expression of which its type or contents are not supported by a method.
    /// </summary>
    [Serializable]
    public class ExpressionNotSupportedException : ArgumentException
    {
        private const string _ExpressionKey = "_expression";
        private readonly string _expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotSupportedException" /> class.
        /// </summary>
        /// <param name="expression">The expression that was not supported.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        public ExpressionNotSupportedException(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            _expression = expression.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotSupportedException" /> class.
        /// </summary>
        /// <param name="expression">The expression that was not supported.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        public ExpressionNotSupportedException(Expression expression, string message)
            : base(message)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            _expression = expression.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotSupportedException" /> class.
        /// </summary>
        /// <param name="expression">The expression that was not supported.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        public ExpressionNotSupportedException(Expression expression, string message, Exception innerException)
            : base(message, innerException)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            _expression = expression.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotSupportedException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ExpressionNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _expression = info.GetString(_ExpressionKey);
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_ExpressionKey, _expression);
        }

        /// <summary>
        /// The expression that was not supported in string-form.
        /// </summary>
        public string Expression
        {
            get { return _expression; }
        }
    }
}
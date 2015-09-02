using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a builder that can be used to build new instances of the <see cref="DataErrorInfo" /> class.
    /// </summary>
    public class DataErrorInfoBuilder : IErrorMessageConsumer
    {                
        private readonly Dictionary<string, string> _errors;        

        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorInfoBuilder" /> class.
        /// </summary>        
        public DataErrorInfoBuilder()
        {            
            _errors = new Dictionary<string, string>();            
        }

        /// <inheritdoc />
        public void Add(string memberName, string errorMessage)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
            _errors.Add(memberName, errorMessage);
        }     

        /// <summary>
        /// Add the specified error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessageFormat">The error-message for this member.</param>
        /// <param name="arg0">The first argument for <paramref name="errorMessageFormat"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, string errorMessageFormat, object arg0)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            Add(memberName, string.Format(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Add the specified error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessageFormat">The error-message for this member.</param>
        /// <param name="arg0">The first argument for <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument for <paramref name="errorMessageFormat"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, string errorMessageFormat, object arg0, object arg1)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            Add(memberName, string.Format(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Add the specified error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessageFormat">The error-message for this member.</param>
        /// <param name="arguments">The arguments for <paramref name="errorMessageFormat"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, string errorMessageFormat, params object[] arguments)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            Add(memberName, string.Format(errorMessageFormat, arguments));
        }                        

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DataErrorInfo" /> class
        /// containing all specified errors and child-errors.
        /// </summary>
        /// <returns>A new instance of the <see cref="DataErrorInfo" /> class.</returns>
        public DataErrorInfo BuildErrorTree()
        {
            return new DataErrorInfo(_errors);
        }        
    }
}

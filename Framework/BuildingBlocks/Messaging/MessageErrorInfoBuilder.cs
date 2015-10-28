﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Kingo.BuildingBlocks.Constraints;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// A builder that can be used to build an instance of the <see cref="MessageErrorInfo" /> class.
    /// </summary>
    public class MessageErrorInfoBuilder : IErrorMessageReader
    {
        private readonly IFormatProvider _formatProvider;
        private readonly Lazy<IDictionary<string, string>> _errorMessages;
        private string _errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageErrorInfoBuilder" /> class.
        /// </summary>
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public MessageErrorInfoBuilder(IFormatProvider formatProvider = null)
        {
            _formatProvider = formatProvider ?? CultureInfo.CurrentCulture;
            _errorMessages = new Lazy<IDictionary<string, string>>(CreateErrorMessageDictionary);
        }

        /// <summary>
        /// Returns the <see cref="IFormatProvider" /> that is used by this builder to format error messages.
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
        }        

        #region [====== Put & Add ======]

        /// <inheritdoc />
        public void Put(IErrorMessage errorMessage)
        {
            Put(Format(errorMessage));
        }

        /// <inheritdoc />
        public void Put(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        /// <inheritdoc />
        public void Add(string memberName, IErrorMessage errorMessage)
        {
            Add(memberName, Format(errorMessage));
        }

        /// <inheritdoc />
        public void Add(string memberName, string errorMessage)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            if (_errorMessages.Value.ContainsKey(memberName))
            {
                return;
            }
            _errorMessages.Value.Add(memberName, errorMessage);
        }
        
        private string Format(IErrorMessage errorMessage)
        {
            return errorMessage == null ? null : errorMessage.ToString(FormatProvider);
        }

        #endregion

        /// <summary>
        /// Creates and returns a new <see cref="MessageErrorInfo"/> instance containing all added error messages.
        /// </summary>
        /// <returns>A new <see cref="MessageErrorInfo"/> instance.</returns>
        public MessageErrorInfo BuildDataErrorInfo()
        {
            return BuildDataErrorInfo(_errorMessages.Value, _errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="MessageErrorInfo"/> instance containing all added error messages.
        /// </summary>
        /// <returns>A new <see cref="MessageErrorInfo"/> instance.</returns>
        protected virtual MessageErrorInfo BuildDataErrorInfo(IDictionary<string, string> errorMessages, string errorMessage)
        {
            return new MessageErrorInfo(errorMessages, errorMessage);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IDictionary{T, S}" /> that will be used to store all error messages.
        /// </summary>
        /// <returns>A new <see cref="IDictionary{T, S}" />.</returns>
        protected virtual IDictionary<string, string> CreateErrorMessageDictionary()
        {
            return new Dictionary<string, string>();
        }
    }
}

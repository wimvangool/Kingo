using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a command sent from a client that lies on top of a regular <see cref="ICommand">Command</see>.
    /// </summary>
    /// <remarks>
    /// Use this class as a base-class for your commands if you don't use any extra execution-parameter.
    /// </remarks>
    public abstract class ClientCommand : ClientCommand<object>        
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientCommand(ICommand request)
            : base(request) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientCommand(ICommand request, IIsValidIndicator isValidIndicator)
            : base(request, isValidIndicator) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommand" /> class.
        /// </summary>
        /// <param name="request">The encapsulated command.</param>
        /// <param name="isValidIndicator">
        /// The indicator used to indicate whether or not the associated <paramref name="request"/> is valid (optional).
        /// </param>
        /// <param name="options">
        /// The execution options that determine the behavior of this request (optional).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>
        protected ClientCommand(ICommand request, IIsValidIndicator isValidIndicator, ClientRequestExecutionOptions options)
            : base(request, isValidIndicator, options) { }

        /// <summary>
        /// Simply assigns <paramref name="parameterIn"/> to <paramref name="parameterOut"/> and returns <c>true</c>.
        /// </summary>
        /// <param name="parameterIn">The incoming parameter.</param>
        /// <param name="parameterOut">
        /// When this method has executed, refers to the same object as <paramref name="parameterIn"/>.
        /// </param>
        /// <returns><c>true</c></returns>
        protected override bool TryConvertParameter(object parameterIn, out object parameterOut)
        {
            parameterOut = parameterIn;
            return true;
        }
    }
}

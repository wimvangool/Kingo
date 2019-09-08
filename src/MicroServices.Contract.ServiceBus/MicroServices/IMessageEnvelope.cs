using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an envelope of a message.
    /// </summary>
    public interface IMessageEnvelope
    {
        /// <summary>
        /// Returns the contents of the message.
        /// </summary>
        object Content
        {
            get;
        }
    }
}

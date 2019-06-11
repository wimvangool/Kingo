using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a message that is being handled or executed
    /// by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    public interface IMessage<out TMessage> : IMessage
    {
        /// <summary>
        /// Returns the message instance.
        /// </summary>
        new TMessage Instance
        {
            get;
        }
    }
}

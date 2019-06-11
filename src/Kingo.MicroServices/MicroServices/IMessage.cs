using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a message that is being handled or executed
    /// by a <see cref="IMicroProcessor" />.
    /// </summary>
    public interface IMessage : IMessageType
    {
        /// <summary>
        /// Returns the message instance.
        /// </summary>
        object Instance
        {
            get;
        }        
    }
}

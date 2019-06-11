using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a specific message type and kind.
    /// </summary>
    public interface IMessageType : ITypeAttributeProvider
    {
        /// <summary>
        /// Indicates whether this message represents a command, event or request.
        /// </summary>
        MessageKind Kind
        {
            get;
        }
    }
}

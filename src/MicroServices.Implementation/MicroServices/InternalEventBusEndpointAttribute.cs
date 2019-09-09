using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to an implementation of the <see cref="IMessageHandler{TMessage}.HandleAsync"/>
    /// method, indicates that the message-handler will receive events from the internal processor
    /// bus, thereby handling them in the same (logical) transaction as these events were published in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InternalEventBusEndpointAttribute : Attribute { }
}

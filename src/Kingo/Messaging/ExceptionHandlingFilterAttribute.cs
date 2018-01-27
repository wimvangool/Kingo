using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that are designed to handle and/or log <see cref="Exception">exceptions</see>
    /// that are thrown while a message is being processed.
    /// </summary>
    public abstract class ExceptionHandlingFilterAttribute : MicroProcessorFilterAttribute
    {
        internal override void Accept(IMicroProcessorFilterAttributeVisitor visitor) =>
            visitor.Visit(this);
    }
}

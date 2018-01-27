namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that are designed to execute just before the message is dispatched to a
    /// message handler or query.
    /// </summary>
    public abstract class ProcessingFilterAttribute : MicroProcessorFilterAttribute
    {
        internal override void Accept(IMicroProcessorFilterAttributeVisitor visitor) =>
            visitor.Visit(this);
    }
}

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that are designed to validate a message before it is being processed.
    /// </summary>
    public abstract class ValidationFilterAttribute : MicroProcessorFilterAttribute
    {
        internal override void Accept(IMicroProcessorFilterAttributeVisitor visitor) =>
            visitor.Visit(this);

        /// <inheritdoc />
        protected internal override IMicroProcessorFilter CreateFilterPipeline() =>
            new RequiresMessageSourceFilter(this, MessageSources.InputStream | MessageSources.Query);
    }
}

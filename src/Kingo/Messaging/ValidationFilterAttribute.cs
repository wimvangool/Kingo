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
        protected override Pipeline CreateFilterPipeline() =>
            base.CreateFilterPipeline().Add(filter => new RequiresMessageSourceFilter(filter, MessageSources.InputStream | MessageSources.Query));
    }
}

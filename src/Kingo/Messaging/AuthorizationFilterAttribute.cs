using Kingo.Messaging.Authorization;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that are designed to authorize the request and/or message that is being processed.
    /// </summary>
    public abstract class AuthorizationFilterAttribute : MicroProcessorFilterAttribute
    {        
        internal override void Accept(IMicroProcessorFilterAttributeVisitor visitor) =>
            visitor.Visit(this);

        /// <inheritdoc />
        protected override Pipeline CreateFilterPipeline(Pipeline pipeline)
        {
            return pipeline
                .Add(filter => new RequiresMessageSourceFilter(filter, MessageSources.InputStream | MessageSources.Query))
                .Add(filter => new RequiresAuthenticatedPrincipalFilter(filter));
        }                 
    }
}

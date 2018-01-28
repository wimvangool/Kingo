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
        protected internal override IMicroProcessorFilter CreateFilterPipeline()
        {            
            IMicroProcessorFilter pipeline = new RequiresAuthenticatedPrincipalFilter(this);
            pipeline = new RequiresMessageSourceFilter(pipeline, MessageSources.InputStream | MessageSources.Query);
            return pipeline;
        }            
    }
}

using Kingo.Messaging.Authorization;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that are designed to authorize the request and/or message that is being processed.
    /// </summary>
    public abstract class AuthorizationFilterAttribute : MicroProcessorFilterAttribute
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationFilterAttribute" /> class.
        /// </summary>
        protected AuthorizationFilterAttribute()
        {
            Sources = MessageSources.InputStream | MessageSources.Query;
        }

        internal override void Accept(IMicroProcessorFilterAttributeVisitor visitor) =>
            visitor.Visit(this);

        /// <inheritdoc />
        protected override FilterPipeline CreateFilterPipeline() =>
            base.CreateFilterPipeline().Add(filter => new RequiresAuthenticatedPrincipalFilter(filter));
    }
}

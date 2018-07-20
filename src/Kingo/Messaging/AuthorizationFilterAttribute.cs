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
            OperationTypes = MicroProcessorOperationTypes.Input;
        }

        internal override void Accept(IMicroProcessorFilterAttributeVisitor visitor) =>
            visitor.Visit(this);        
    }
}

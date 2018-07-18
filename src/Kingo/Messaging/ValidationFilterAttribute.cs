namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that are designed to validate a message before it is being processed.
    /// </summary>
    public abstract class ValidationFilterAttribute : MicroProcessorFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationFilterAttribute" /> class.
        /// </summary>
        protected ValidationFilterAttribute()
        {
            Sources = MessageSources.Input;
        }

        internal override void Accept(IMicroProcessorFilterAttributeVisitor visitor) =>
            visitor.Visit(this);        
    }
}

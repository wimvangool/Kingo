namespace System.ComponentModel.Server
{
    /// <summary>
    /// This attribute must be put on each <see cref="IMessageHandler{T}" /> class to support auto-registration of it
    /// by the <see cref="MessageHandlerFactory" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MessageHandlerAttribute : Attribute
    {
        private readonly InstanceLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute" /> class.
        /// </summary>
        /// <param name="lifetime">The lifetime of the <see cref="IMessageHandler{T}" />.</param>
        public MessageHandlerAttribute(InstanceLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        /// <summary>
        /// The lifetime of the <see cref="IMessageHandler{T}" />.
        /// </summary>
        public InstanceLifetime Lifetime
        {
            get { return _lifetime; }
        }
    }
}

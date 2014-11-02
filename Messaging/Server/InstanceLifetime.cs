
namespace System.ComponentModel.Server
{
    /// <summary>
    /// A value that is used to specify the lifetime of a certain message-handler at runtime.
    /// </summary>
    public enum InstanceLifetime
    {
        /// <summary>
        /// Specifies that a new instance should be created each time a new message is handled.
        /// </summary>
        /// <remarks>
        /// When this lifetime-mode is specified, a new instance of the handler is created each time
        /// it handles a new message (command or domain-event). This lifetime-mode is similar to the PerResolve
        /// lifetime mode of a WCF-service.
        /// </remarks>
        PerResolve,

        /// <summary>
        /// Specifies that only one instance of the handler is to be created during execution of a single command.        
        /// </summary>
        /// <remarks>
        /// When this lifetime mode is specified, only one instance of the handler is created for each command
        /// that is executed. If the handler handles multiple messages (such as multiple domain-events), a single
        /// instance will receive these events. This lifetime-mode is similar to the PerSession-lifetime mode
        /// of a WCF-service.
        /// </remarks>
        PerUnitOfWork,

        /// <summary>
        /// Specifies that only one instance of the handler is to be created by the container it was registered in.
        /// </summary>
        /// <remarks>
        /// When this lifetime mode is specified, the container that has registered the handler will always return
        /// the same instance. This will typically result in a singleton-behavior of the handler. This lifetime mode
        /// is therefore similar to the Single-lifetime mode of a WCF-service.      
        /// </remarks>
        Single
    }
}

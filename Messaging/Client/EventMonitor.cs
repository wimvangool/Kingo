using System.ComponentModel.Server;
using System.Runtime.Caching;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a <see cref="ChangeMonitor" /> that connects to a <see cref="IMessageProcessorBus" /> to receive
    /// messages that (may) signal that the cached value has expired.
    /// </summary>
    public abstract class EventMonitor : ChangeMonitor
    {        
        private readonly IConnection _connection;
        private readonly string _uniqueId;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMonitor" /> class.
        /// </summary>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventBus"/> is <c>null</c>.
        /// </exception>    
        protected EventMonitor(IMessageProcessorBus eventBus)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException("eventBus");
            }
            // Instructions for custom ChangeMonitors say that monitors
            // must start monitoring in their constructor and before
            // InitializationComplete() is called. Also, if something goes
            // wrong here, InitializationComplete() has to
            // be called before Dispose() is called.
            try
            {
                _connection = eventBus.Connect(this, true);
                _uniqueId = Guid.NewGuid().ToString();
            }
            catch
            {
                InitializationComplete();
                Dispose();
                throw;
            }
            InitializationComplete();                        
        }

        /// <inheritdoc />
        public override string UniqueId
        {
            get { return _uniqueId; }
        }                

        #region [====== Dispose ======]                

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing && _connection != null)
            {
                _connection.Dispose();
            }            
        }        

        #endregion   
     
        #region [====== OnChanged ======]                               

        /// <summary>
        /// Called by derived classes to raise the event when a dependency changes.
        /// </summary>
        protected void OnChanged()
        {
            OnChanged(null);
        }

        #endregion
    }
}

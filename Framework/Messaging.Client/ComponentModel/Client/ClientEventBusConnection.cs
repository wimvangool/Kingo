namespace Syztem.ComponentModel.Client
{
    internal sealed class ClientEventBusConnection : Connection
    {
        private readonly ClientEventBus _eventBus;
        private readonly object _subscriber;
        private bool _isOpen;        

        public ClientEventBusConnection(ClientEventBus eventBus, object subscriber)
        {
            _eventBus = eventBus;
            _subscriber = subscriber;
        }        

        #region [====== Opening and Closing ======]

        protected override bool IsOpen
        {
            get { return _isOpen; }
        }

        protected override void OpenConnection()
        {
            _eventBus.Subscribe(_subscriber);
            _isOpen = true;
        }

        protected override void CloseConnection()
        {
            _eventBus.Unsubscribe(_subscriber);
            _isOpen = false;
        }        

        #endregion        
    }
}

using System.Collections.Generic;

namespace Syztem.ComponentModel.Server
{
    internal sealed class CompositeConnection : Connection
    {
        private readonly List<IConnection> _connections;
        private bool _isOpen;

        public CompositeConnection(IEnumerable<IConnection> connections)
        {
            _connections = new List<IConnection>(connections);
            _connections.TrimExcess();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                foreach (var connection in _connections)
                {
                    connection.Dispose();
                }
                _isOpen = false;
            }
            base.Dispose(disposing);
        }

        protected override bool IsOpen
        {
            get { return _isOpen; }
        }

        protected override void OpenConnection()
        {
            foreach (var connection in _connections)
            {
                connection.Open();
            }
            _isOpen = true;
        }

        protected override void CloseConnection()
        {
            foreach (var connection in _connections)
            {
                connection.Close();
            }
            _isOpen = false;
        }        
    }
}

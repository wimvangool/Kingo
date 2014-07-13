using System;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    internal abstract class Connection : IConnection
    {
        #region [====== Dispose ======]

        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            if (IsOpen)
            {
                Close();
            }
            _isDisposed = true;
        }

        #endregion

        #region [====== Opening and Closing ======]

        protected abstract bool IsOpen
        {
            get;
        }

        public void Open()
        {
            if (_isDisposed)
            {
                throw NewConnectionAlreadyDisposedException(this);
            }
            if (IsOpen)
            {
                throw NewConnectionAlreadyOpenException();
            }
            OpenConnection();
        }

        protected abstract void OpenConnection();        
        
        public void Close()
        {
            if (_isDisposed)
            {
                throw NewConnectionAlreadyDisposedException(this);
            }
            if (IsOpen)
            {
                CloseConnection();
                return;
            }
            throw NewConnectionAlreadyClosedException();
        }        

        protected abstract void CloseConnection();

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewConnectionAlreadyDisposedException(object connection)
        {
            return new ObjectDisposedException(connection.GetType().Name);
        }

        private static Exception NewConnectionAlreadyOpenException()
        {
            return new InvalidOperationException(ExceptionMessages.Connection_AlreadyOpen);
        }

        private static Exception NewConnectionAlreadyClosedException()
        {
            return new InvalidOperationException(ExceptionMessages.Connection_AlreadyClosed);
        }

        #endregion
    }
}

using System;
using Kingo.Threading;

namespace Kingo.Samples.Chess
{
    public sealed class Session
    {        
        private readonly string _playerName;

        private Session(string playerName)
        {
            _playerName = playerName;
        }

        public string PlayerName
        {
            get { return _playerName; }
        }

        #region [====== Current ======]

        private static readonly Context<Session> _Context = new Context<Session>();

        public static Session Current
        {
            get { return _Context.Current; }
        }

        public static IDisposable CreateSession(string playerName)
        {
            if (playerName == null)
            {
                throw new ArgumentNullException("playerName");
            }
            return _Context.OverrideAsyncLocal(new Session(playerName));
        }

        #endregion
    }
}

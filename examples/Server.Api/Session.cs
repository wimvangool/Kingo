using System;
using System.Runtime.Serialization;
using Kingo.Threading;

namespace Kingo.Samples.Chess
{
    [DataContract]
    public sealed class Session
    {
        public static readonly string HeaderName = typeof(Session).Name;
        public static readonly string HeaderNamespace = typeof(Session).Namespace;

        [DataMember]
        private readonly Guid _playerId;

        [DataMember]
        private readonly string _playerName;

        public Session(Guid playerId, string playerName)
        {
            if (playerName == null)
            {
                throw new ArgumentNullException(nameof(playerName));
            }
            _playerId = playerId;
            _playerName = playerName;
        }

        public Guid PlayerId
        {
            get { return _playerId; }
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

        public static IDisposable CreateSessionScope(Guid playerId, string playerName)
        {            
            return CreateSessionScope(new Session(playerId, playerName));
        }

        public static IDisposable CreateSessionScope(Session session)
        {
            return _Context.OverrideAsyncLocal(session);
        }

        #endregion        
    }
}

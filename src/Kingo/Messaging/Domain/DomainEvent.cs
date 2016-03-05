using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for events that are published by aggregates.
    /// </summary>
    [Serializable]   
    public abstract class DomainEvent : Message
    {        
        internal DomainEvent() { }

        #region [====== Keys (Get) ======]

        private static readonly ConcurrentDictionary<Type, Delegate> _GetKeyDelegates = new ConcurrentDictionary<Type, Delegate>();

        internal TKey GetKey<TKey>()
        {
            return GetKeyDelegate<TKey>().Invoke(this);
        }

        private Func<object, TKey> GetKeyDelegate<TKey>()
        {
            return _GetKeyDelegates.GetOrAdd(GetType(), CreateGetKeyDelegate<TKey>) as Func<object, TKey>;
        }

        private static Func<object, TKey> CreateGetKeyDelegate<TKey>(Type messageType)
        {
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var keyMember = new DomainEventKeyMember(typeof(TKey));
            var keyAccessExpression = keyMember.CreateMemberAccessExpression(message);

            return Expression.Lambda<Func<object, TKey>>(keyAccessExpression, messageParameter).Compile();
        }        

        #endregion

        #region [====== Keys (Set) ======]

        private static readonly ConcurrentDictionary<Type, Delegate> _SetKeyDelegates = new ConcurrentDictionary<Type, Delegate>();

        internal void SetKey<TKey>(TKey key)
        {
            SetKeyDelegate<TKey>().Invoke(this, key);
        }

        private Action<object, TKey> SetKeyDelegate<TKey>()
        {
            return _SetKeyDelegates.GetOrAdd(GetType(), CreateSetKeyDelegate<TKey>) as Action<object, TKey>;
        }

        private static Action<object, TKey> CreateSetKeyDelegate<TKey>(Type messageType)
        {
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var keyParameter = Expression.Parameter(typeof(TKey), "key");
            var keyMember = new DomainEventKeyMember(typeof(TKey));
            var keyAssignmentExpression = keyMember.CreateMemberAssignmentExpression(message, keyParameter);

            return Expression.Lambda<Action<object, TKey>>(keyAssignmentExpression, messageParameter, keyParameter).Compile();
        }

        #endregion                      

        #region [====== Versions (Get) ======]

        private static readonly ConcurrentDictionary<Type, Delegate> _GetVersionDelegates = new ConcurrentDictionary<Type, Delegate>();

        internal TVersion GetVersion<TVersion>()
        {
            return GetVersionDelegate<TVersion>().Invoke(this);
        }

        private Func<object, TVersion> GetVersionDelegate<TVersion>()
        {
            return _GetVersionDelegates.GetOrAdd(GetType(), CreateGetVersionDelegate<TVersion>) as Func<object, TVersion>;
        }

        private static Func<object, TVersion> CreateGetVersionDelegate<TVersion>(Type messageType)
        {
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var versionMember = new DomainEventVersionMember(typeof(TVersion));
            var versionAccessExpression = versionMember.CreateMemberAccessExpression(message);

            return Expression.Lambda<Func<object, TVersion>>(versionAccessExpression, messageParameter).Compile();
        }        

        #endregion

        #region [====== Versions (Set) ======]

        private static readonly ConcurrentDictionary<Type, Delegate> _SetVersionDelegates = new ConcurrentDictionary<Type, Delegate>();

        internal void SetVersion<TVersion>(TVersion version)
        {
            SetVersionDelegate<TVersion>().Invoke(this, version);
        }

        private Action<object, TVersion> SetVersionDelegate<TVersion>()
        {
            return _SetVersionDelegates.GetOrAdd(GetType(), CreateSetVersionDelegate<TVersion>) as Action<object, TVersion>;
        }

        private static Action<object, TVersion> CreateSetVersionDelegate<TVersion>(Type messageType)
        {
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var versionParameter = Expression.Parameter(typeof(TVersion), "version");
            var versionMember = new DomainEventVersionMember(typeof(TVersion));
            var versionAssignmentExpression = versionMember.CreateMemberAssignmentExpression(message, versionParameter);

            return Expression.Lambda<Action<object, TVersion>>(versionAssignmentExpression, messageParameter, versionParameter).Compile();
        }

        #endregion       
    }
}

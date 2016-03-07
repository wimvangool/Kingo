using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for events that are published by an application.
    /// </summary>
    [Serializable]   
    public abstract class DomainEvent : Message, IDomainEvent
    {
        #region [====== Instance Members ======]

        IDomainEvent IDomainEvent.UpgradeToLatestVersion()
        {
            return UpgradeToLatestVersion();
        }

        /// <summary>
        /// Upgrades this event to the latest version.
        /// </summary>
        /// <returns>The latest version of this event.</returns>
        protected virtual IDomainEvent UpgradeToLatestVersion()
        {
            return this;
        }

        #endregion

        #region [====== Keys (Get) ======]

        private static readonly ConcurrentDictionary<Type, Delegate> _GetKeyDelegates = new ConcurrentDictionary<Type, Delegate>();

        internal static TKey GetKey<TKey>(object message)
        {
            return GetKeyDelegate<TKey>(message.GetType()).Invoke(message);
        }

        private static Func<object, TKey> GetKeyDelegate<TKey>(Type messageType)
        {
            return _GetKeyDelegates.GetOrAdd(messageType, CreateGetKeyDelegate<TKey>) as Func<object, TKey>;
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

        internal static void SetKey<TKey>(object message, TKey key)
        {
            SetKeyDelegate<TKey>(message.GetType()).Invoke(message, key);
        }

        private static Action<object, TKey> SetKeyDelegate<TKey>(Type messageType)
        {
            return _SetKeyDelegates.GetOrAdd(messageType, CreateSetKeyDelegate<TKey>) as Action<object, TKey>;
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

        internal static TVersion GetVersion<TVersion>(object message)
        {
            return GetVersionDelegate<TVersion>(message.GetType()).Invoke(message);
        }

        private static Func<object, TVersion> GetVersionDelegate<TVersion>(Type messageType)
        {
            return _GetVersionDelegates.GetOrAdd(messageType, CreateGetVersionDelegate<TVersion>) as Func<object, TVersion>;
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

        internal static void SetVersion<TVersion>(object message, TVersion version)
        {
            SetVersionDelegate<TVersion>(message.GetType()).Invoke(message, version);
        }

        private static Action<object, TVersion> SetVersionDelegate<TVersion>(Type messageType)
        {
            return _SetVersionDelegates.GetOrAdd(messageType, CreateSetVersionDelegate<TVersion>) as Action<object, TVersion>;
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

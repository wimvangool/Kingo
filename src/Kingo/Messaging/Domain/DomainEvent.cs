using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for events that are published by aggregates.
    /// </summary>
    [Serializable]   
    public abstract class DomainEvent : Message
    {
        private const BindingFlags _MemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        #region [====== Keys ======]

        private static readonly ConcurrentDictionary<Type, Delegate> _KeyDelegates = new ConcurrentDictionary<Type, Delegate>();

        internal TKey GetKey<TKey>()
        {
            try
            {
                return GetKeyDelegate<TKey>().Invoke(this);
            }
            catch (NullReferenceException)
            {
                throw NewIncompatibleKeyTypeException(GetType());
            }
            catch (InvalidCastException)
            {
                throw NewIncompatibleKeyTypeException(GetType());
            }            
        }

        private Func<object, TKey> GetKeyDelegate<TKey>()
        {
            return _KeyDelegates.GetOrAdd(GetType(), CreateKeyDelegate<TKey>) as Func<object, TKey>;
        }

        private static Func<object, TKey> CreateKeyDelegate<TKey>(Type messageType)
        {
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var keyAccessExpression = CreateKeyAccessExpression(message, typeof(TKey));

            return Expression.Lambda<Func<object, TKey>>(keyAccessExpression, messageParameter).Compile();
        }

        private static Expression CreateKeyAccessExpression(Expression message, Type keyType)
        {
            var fieldsMarkedAsKey = FindFieldsMarkedAsKey(message.Type);
            var propertiesMarkedAsKey = FindPropertiesMarkedAsKey(message.Type);

            var keyMemberCount = fieldsMarkedAsKey.Length + propertiesMarkedAsKey.Length;
            if (keyMemberCount == 0)
            {
                throw NewKeyNotFoundException(message.Type);
            }
            if (keyMemberCount > 1)
            {
                throw NewMultipleKeyCandidatesException(message.Type);
            }
            if (fieldsMarkedAsKey.Length == 1)
            {
                return CreateFieldAccessExpression(message, fieldsMarkedAsKey[0], keyType);
            }
            return CreatePropertyAccessExpression(message, propertiesMarkedAsKey[0], keyType);
        }       

        private static FieldInfo[] FindFieldsMarkedAsKey(Type type)
        {
            var fields =
                from field in type.GetFields(_MemberFlags)
                where IsKeyMember(field)
                select field;

            return fields.ToArray();
        }

        private static PropertyInfo[] FindPropertiesMarkedAsKey(Type type)
        {
            var properties =
                from property in type.GetProperties(_MemberFlags)
                where IsKeyMember(property)
                select property;

            return properties.ToArray();
        }

        private static bool IsKeyMember(MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(KeyAttribute)).Any();
        }

        private static Exception NewKeyNotFoundException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_KeyMemberNotFound;
            var message = string.Format(messageFormat, domainEventType);
            return new InvalidOperationException(message);
        }        

        private static Exception NewMultipleKeyCandidatesException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_MultipleKeyMembersFound;
            var message = string.Format(messageFormat, domainEventType);
            return new InvalidOperationException(message);
        }

        private static Exception NewIncompatibleKeyTypeException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_IncompatibleKeyType;
            var message = string.Format(messageFormat, domainEventType);
            return new InvalidOperationException(message);
        }

        #endregion

        #region [====== Versions ======]

        private static readonly ConcurrentDictionary<Type, Delegate> _VersionDelegates = new ConcurrentDictionary<Type, Delegate>();

        internal TVersion GetVersion<TVersion>()
        {
            try
            {
                return GetVersionDelegate<TVersion>().Invoke(this);
            }
            catch (NullReferenceException)
            {
                throw NewIncompatibleVersionTypeException(GetType());
            }
            catch (InvalidCastException)
            {
                throw NewIncompatibleVersionTypeException(GetType());
            }  
        }

        private Func<object, TVersion> GetVersionDelegate<TVersion>()
        {
            return _VersionDelegates.GetOrAdd(GetType(), CreateVersionDelegate<TVersion>) as Func<object, TVersion>;
        }

        private static Func<object, TVersion> CreateVersionDelegate<TVersion>(Type messageType)
        {
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var versionAccessExpression = CreateVersionAccessExpression(message, typeof(TVersion));

            return Expression.Lambda<Func<object, TVersion>>(versionAccessExpression, messageParameter).Compile();
        }

        private static Expression CreateVersionAccessExpression(Expression message, Type versionType)
        {
            var fieldsMarkedAsVersion = FindFieldsMarkedAsVersion(message.Type);
            var propertiesMarkedAsVersion = FindPropertiesMarkedAsVersion(message.Type);

            var versionMemberCount = fieldsMarkedAsVersion.Length + propertiesMarkedAsVersion.Length;
            if (versionMemberCount == 0)
            {
                throw NewVersionNotFoundException(message.Type);
            }
            if (versionMemberCount > 1)
            {
                throw NewMultipleVersionCandidatesException(message.Type);
            }
            if (fieldsMarkedAsVersion.Length == 1)
            {
                return CreateFieldAccessExpression(message, fieldsMarkedAsVersion[0], versionType);
            }
            return CreatePropertyAccessExpression(message, propertiesMarkedAsVersion[0], versionType);
        }

        private static FieldInfo[] FindFieldsMarkedAsVersion(Type type)
        {
            var fields =
                from field in type.GetFields(_MemberFlags)
                where IsVersionMember(field)
                select field;

            return fields.ToArray();
        }

        private static PropertyInfo[] FindPropertiesMarkedAsVersion(Type type)
        {
            var properties =
                from property in type.GetProperties(_MemberFlags)
                where IsVersionMember(property)
                select property;

            return properties.ToArray();
        }

        private static bool IsVersionMember(MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(VersionAttribute)).Any();
        }

        private static Exception NewVersionNotFoundException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_VersionMemberNotFound;
            var message = string.Format(messageFormat, domainEventType);
            return new InvalidOperationException(message);
        }

        private static Exception NewMultipleVersionCandidatesException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_MultipleVersionMembersFound;
            var message = string.Format(messageFormat, domainEventType);
            return new InvalidOperationException(message);
        }

        private static Exception NewIncompatibleVersionTypeException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_IncompatibleVersionType;
            var message = string.Format(messageFormat, domainEventType);
            return new InvalidOperationException(message);
        }

        #endregion

        private static Expression CreateFieldAccessExpression(Expression message, FieldInfo field, Type memberType)
        {
            Expression fieldAccessExpression = Expression.Field(message, field);

            if (field.FieldType != memberType)
            {
                fieldAccessExpression = Expression.Convert(fieldAccessExpression, memberType);
            }
            return fieldAccessExpression;
        }

        private static Expression CreatePropertyAccessExpression(Expression message, PropertyInfo property, Type memberType)
        {
            Expression propertyAccessExpression = Expression.Property(message, property);

            if (property.PropertyType != memberType)
            {
                propertyAccessExpression = Expression.Convert(propertyAccessExpression, memberType);
            }
            return propertyAccessExpression;
        }
    }
}

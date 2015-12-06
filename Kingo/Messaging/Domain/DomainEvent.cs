using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    internal static class DomainEvent
    {
        #region [====== Member ======]

        private abstract class Member<T>
        {
            protected abstract Type Type
            {
                get;
            }

            protected abstract string Name
            {
                get;
            }

            internal T Invoke(object domainEvent)
            {
                var value = InvokeMember(domainEvent);

                try
                {
                    return (T) value;
                }
                catch (NullReferenceException)
                {
                    throw NewIncompatibleMemberTypeException(domainEvent.GetType(), Name, Type);
                }
                catch (InvalidCastException)
                {
                    throw NewIncompatibleMemberTypeException(domainEvent.GetType(), Name, Type);
                }
            }

            protected abstract object InvokeMember(object domainEvent);

            private static Exception NewIncompatibleMemberTypeException(Type domainEventType, string name, Type type)
            {
                var messageFormat = ExceptionMessages.DomainEvent_IncompatibleMemberType;
                var message = string.Format(messageFormat, domainEventType.Name, name, type, typeof(T));
                return new InvalidOperationException(message);
            }
        }

        private sealed class FieldMember<T> : Member<T>
        {
            private readonly FieldInfo _field;

            internal FieldMember(FieldInfo field)
            {
                _field = field;
            }

            protected override Type Type
            {
                get { return _field.FieldType; }
            }

            protected override string Name
            {
                get { return _field.Name; }
            }

            protected override object InvokeMember(object domainEvent)
            {
                return _field.GetValue(domainEvent);
            }
        }

        private sealed class PropertyMember<T> : Member<T>
        {
            private readonly PropertyInfo _property;

            internal PropertyMember(PropertyInfo property)
            {
                if (property.GetIndexParameters().Length > 0)
                {
                    throw NewIndexerNotSupportedException(property);
                }
                _property = property;
            }            

            protected override Type Type
            {
                get { return _property.PropertyType; }
            }

            protected override string Name
            {
                get { return _property.Name; }
            }

            protected override object InvokeMember(object domainEvent)
            {
                return _property.GetValue(domainEvent);
            }

            private static Exception NewIndexerNotSupportedException(PropertyInfo property)
            {
                var messageFormat = ExceptionMessages.DomainEvent_IndexerNotSupported;
                var message = string.Format(messageFormat, typeof(AggregateMemberAttribute).Name, property.DeclaringType.Name, property.Name);
                return new InvalidOperationException(message);
            }
        }

        #endregion

        private const BindingFlags _MemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly ConcurrentDictionary<Type, object> _KeyMembers = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, object> _VersionMembers = new ConcurrentDictionary<Type, object>();

        #region [====== Key ======]

        internal static TKey GetKey<TKey>(object domainEvent)
        {
            return GetOrAddKeyMember<TKey>(domainEvent.GetType()).Invoke(domainEvent);
        }

        private static Member<TKey> GetOrAddKeyMember<TKey>(Type domainEventType)
        {
            return _KeyMembers.GetOrAdd(domainEventType, type => GetMember<TKey>(type, AggregateMemberType.Key)) as Member<TKey>;
        }        

        #endregion

        #region [====== Version ======]

        internal static TVersion GetVersion<TVersion>(object domainEvent)
        {
            return GetOrAddVersionMember<TVersion>(domainEvent.GetType()).Invoke(domainEvent);
        }

        private static Member<TVersion> GetOrAddVersionMember<TVersion>(Type domainEventType)
        {
            return _VersionMembers.GetOrAdd(domainEventType, type => GetMember<TVersion>(type, AggregateMemberType.Version)) as Member<TVersion>;
        }        

        #endregion

        private static Member<TKey> GetMember<TKey>(Type domainEventType, AggregateMemberType memberType)
        {
            Member<TKey> member;

            if (TryGetExplicitMember(domainEventType, memberType, out member))
            {
                return member;
            }
            if (TryGetImplicitMember(domainEventType, memberType, out member))
            {
                return member;
            }
            throw NewMemberNotFoundException(domainEventType, memberType);
        }

        private static bool TryGetExplicitMember<TKey>(Type domainEventType, AggregateMemberType memberType, out Member<TKey> member)
        {            
            var fields =
                from field in domainEventType.GetFields(_MemberFlags)
                where HasAttribute(field, memberType)
                select new FieldMember<TKey>(field);

            var properties =
                from property in domainEventType.GetProperties(_MemberFlags)
                where HasAttribute(property, memberType)
                select new PropertyMember<TKey>(property);

            var members = fields.Cast<Member<TKey>>().Concat(properties).ToArray();
            if (members.Length == 0)
            {
                member = null;
                return false;
            }
            if (members.Length == 1)
            {
                member = members[0];
                return true;
            }
            throw NewMultipleAttributesDeclaredException(domainEventType, memberType);
        }

        private static bool HasAttribute(MemberInfo member, AggregateMemberType memberType)
        {
            var attribute = member.GetCustomAttribute<AggregateMemberAttribute>(true);
            if (attribute == null)
            {
                return false;
            }
            return attribute.MemberType == memberType;
        }

        private static bool TryGetImplicitMember<TKey>(Type domainEventType, AggregateMemberType memberType, out Member<TKey> member)
        {
            var fields =
                from field in domainEventType.GetFields(_MemberFlags)
                where IsMemberCandidate(memberType, field.Name)
                select new FieldMember<TKey>(field);

            var properties =
                from property in domainEventType.GetProperties(_MemberFlags)
                where property.GetIndexParameters().Length == 0
                where IsMemberCandidate(memberType, property.Name)
                select new PropertyMember<TKey>(property);

            var members = fields.Cast<Member<TKey>>().Concat(properties).ToArray();
            if (members.Length == 0)
            {
                member = null;
                return false;
            }
            if (members.Length == 1)
            {
                member = members[0];
                return true;
            }
            throw NewMultipleMemberCandidatesException(domainEventType, memberType);
        }        

        private static bool IsMemberCandidate(AggregateMemberType memberType, string name)
        {
            if (memberType == AggregateMemberType.Key)
            {
                return name.EndsWith("Id") || name.EndsWith("Key");
            }
            if (memberType == AggregateMemberType.Version)
            {
                return name.EndsWith("Version");
            }
            return false;
        }

        private static Exception NewMemberNotFoundException(Type domainEventType, AggregateMemberType memberType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_MemberNotFound;
            var message = string.Format(messageFormat, memberType, domainEventType.Name, typeof(AggregateMemberAttribute).Name);
            return new InvalidOperationException(message);
        }

        private static Exception NewMultipleAttributesDeclaredException(Type domainEventType, AggregateMemberType memberType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_MultipleAttributesDeclared;
            var message = string.Format(messageFormat, memberType, domainEventType.Name);
            return new InvalidOperationException(message);
        }

        private static Exception NewMultipleMemberCandidatesException(Type domainEventType, AggregateMemberType memberType)
        {
            var messageFormat = ExceptionMessages.DomainEvent_MultipleCandidateMembersFound;
            var message = string.Format(messageFormat, memberType, domainEventType.Name, typeof(AggregateMemberAttribute).Name);
            return new InvalidOperationException(message);
        }
    }
}

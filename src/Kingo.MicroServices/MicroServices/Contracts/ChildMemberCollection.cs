using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices.Contracts
{
    internal abstract class ChildMemberCollection : ChildMember
    {
        #region [====== Item<TKey> ======]

        protected abstract class Item<TKeyOrIndex> : ChildMember
        {
            private readonly ChildMember _collectionMember;
            private readonly TKeyOrIndex _keyOrIndex;

            protected Item(ChildMember collectionMember, TKeyOrIndex keyOrIndex)
            {
                _collectionMember = collectionMember;
                _keyOrIndex = keyOrIndex;
            }

            protected TKeyOrIndex KeyOrIndex =>
                _keyOrIndex;

            public override string Name =>
                $"{_collectionMember.Name}[{_keyOrIndex}]";                        
        }

        #endregion

        private readonly ChildMember _member;

        protected ChildMemberCollection(ChildMember member)
        {
            _member = member;
        }

        public override string Name =>
            _member.Name;

        #region [====== FromChildMember ======]        

        private static readonly ConcurrentDictionary<Type, Func<ChildMember, IEnumerable<ChildMemberCollection>>> _CollectionFactories =
            new ConcurrentDictionary<Type, Func<ChildMember, IEnumerable<ChildMemberCollection>>>();

        public static IEnumerable<ChildMemberCollection> FromChildMember(ChildMember member) =>
            _CollectionFactories.GetOrAdd(member.Type, CreateCollectionFactory).Invoke(member);

        private static Func<ChildMember, IEnumerable<ChildMemberCollection>> CreateCollectionFactory(Type memberType)
        {
            return CreateCollections;
        }

        private static IEnumerable<ChildMemberCollection> CreateCollections(ChildMember member)
        {
            foreach (var enumerableType in member.Type.GetInterfacesOfType(typeof(IEnumerable<>)))
            {
                yield return CreateCollection(enumerableType.GetGenericArguments()[0], member);
            }
        }

        private static ChildMemberCollection CreateCollection(Type itemType, ChildMember member)
        {
            var collectionType = CreateCollectionType(itemType);
            var constructor = collectionType.GetConstructor(new [] { typeof(ChildMember) });
            return (ChildMemberCollection) constructor.Invoke(new object[] { member });
        }

        private static Type CreateCollectionType(Type itemType)
        {
            if (itemType.IsValueType && itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var keyType = itemType.GetGenericArguments()[0];
                var valueType = itemType.GetGenericArguments()[1];

                return typeof(ChildMemberDictionary<,>).MakeGenericType(keyType, valueType);
            }
            return typeof(ChildMemberCollection<>).MakeGenericType(itemType);
        }

        #endregion
    }
}

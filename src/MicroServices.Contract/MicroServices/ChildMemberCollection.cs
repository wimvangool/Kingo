using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
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
            var parameter = Expression.Parameter(typeof(ChildMember), "childMember");
            var body = Expression.NewArrayInit(typeof(ChildMemberCollection), CreateCollectionElementExpressions(memberType, parameter));
            return Expression.Lambda<Func<ChildMember, IEnumerable<ChildMemberCollection>>>(body, parameter).Compile();
        }

        private static IEnumerable<Expression> CreateCollectionElementExpressions(Type memberType, Expression childMember) =>
            from enumerableType in memberType.GetInterfacesOfType(typeof(IEnumerable<>))
            select CreateNewCollectionExpression(enumerableType.GetGenericArguments()[0], childMember);

        private static Expression CreateNewCollectionExpression(Type itemType, Expression childMember) =>
            Expression.New(GetCollectionConstructor(itemType), childMember);

        private static ConstructorInfo GetCollectionConstructor(Type itemType) =>
            CreateCollectionType(itemType).GetConstructor(new[] { typeof(ChildMember) });

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

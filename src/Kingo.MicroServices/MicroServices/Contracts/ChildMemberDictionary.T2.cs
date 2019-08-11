using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Kingo.MicroServices.Contracts
{
    internal sealed class ChildMemberDictionary<TKey, TValue> : ChildMemberCollection
    {
        #region [====== Item ======]

        private sealed class Item : Item<TKey>
        {
            public Item(ChildMember collectionMember, TKey key) :
                base(collectionMember, key) { }           

            public override Type Type =>
                typeof(TValue);

            protected override object GetValue(object instance) =>
                GetValue((IReadOnlyDictionary<TKey, TValue>) instance);

            private TValue GetValue(IReadOnlyDictionary<TKey, TValue> items) =>
                items[KeyOrIndex];
        }

        #endregion

        [UsedImplicitly]
        public ChildMemberDictionary(ChildMember member) :
            base(member) { }

        public override Type Type =>
            typeof(IReadOnlyDictionary<TKey, TValue>);

        protected override object GetValue(object instance) =>
            GetValue((IEnumerable<KeyValuePair<TKey, TValue>>) instance);

        private static Dictionary<TKey, TValue> GetValue(IEnumerable<KeyValuePair<TKey, TValue>> instance)
        {
            var items = new Dictionary<TKey, TValue>();

            foreach (var keyValuePair in instance)
            {
                items.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return items;
        }

        protected override IEnumerable<ValidationResult> ValidateChildMember(ValidationContext validationContext) =>
            from item in CreateCollectionItems((IReadOnlyDictionary<TKey, TValue>) validationContext.ObjectInstance)
            from result in item.Validate(validationContext)
            select result;

        private IEnumerable<ChildMember> CreateCollectionItems(IReadOnlyDictionary<TKey, TValue> collection)
        {
            foreach (var key in collection.Keys)
            {
                yield return new Item(this, key);
            }
        }
    }
}

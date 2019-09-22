using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JetBrains.Annotations;

namespace Kingo.MicroServices.DataAnnotations
{
    internal sealed class ChildMemberCollection<TChildMember> : ChildMemberCollection
    {
        #region [====== Item ======]

        private sealed class Item : Item<int>
        {
            public Item(ChildMember collectionMember, int index) :
                base(collectionMember, index) { }            

            public override Type Type =>
                typeof(TChildMember);

            protected override object GetValue(object instance) =>
                GetValue((IReadOnlyList<TChildMember>) instance);

            private TChildMember GetValue(IReadOnlyList<TChildMember> items) =>
                items[KeyOrIndex];
        }

        #endregion        

        [UsedImplicitly]
        public ChildMemberCollection(ChildMember member) :
            base(member) { }

        public override Type Type =>
            typeof(IReadOnlyList<TChildMember>);        

        protected override object GetValue(object instance) =>
            ((IEnumerable<TChildMember>) instance).ToArray();

        protected override IEnumerable<ValidationResult> ValidateChildMember(ValidationContext validationContext) =>
            from item in CreateCollectionItems((IReadOnlyCollection<TChildMember>) validationContext.ObjectInstance)
            from result in item.Validate(validationContext)
            select result;
        
        private IEnumerable<ChildMember> CreateCollectionItems(IReadOnlyCollection<TChildMember> collection)
        {
            for (var index = 0; index < collection.Count; index++)
            {
                yield return new Item(this, index);
            }
        }
    }
}

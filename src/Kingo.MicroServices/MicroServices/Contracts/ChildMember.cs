using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Kingo.Reflection;
using static Kingo.MicroServices.Contracts.ObjectExtensions;

namespace Kingo.MicroServices.Contracts
{
    internal abstract class ChildMember : IValidatableObject
    {
        #region [====== Name & Type ======]                

        public abstract string Name
        {
            get;
        }

        public abstract Type Type
        {
            get;
        }

        public override string ToString() =>
            $"{Name} ({Type.FriendlyName()})";

        #endregion

        #region [====== Validate ======]

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TryCreateChildValidationContext(validationContext, out var childValidationContext))
            {
                return ValidateChildMember(childValidationContext);
            }
            return Enumerable.Empty<ValidationResult>();
        }

        private bool TryCreateChildValidationContext(ValidationContext validationContext, out ValidationContext childValidationContext)
        {
            var childMemberValue = GetValue(validationContext.ObjectInstance);
            if (childMemberValue == null)
            {
                childValidationContext = null;
                return false;
            }            
            childValidationContext = new ValidationContext(childMemberValue, validationContext, validationContext.Items)
            {
                MemberName = Name
            };            
            return true;
        }                                    
        
        protected abstract object GetValue(object instance);

        #endregion

        #region [====== ValidateChildMember ======]

        protected virtual IEnumerable<ValidationResult> ValidateChildMember(ValidationContext validationContext) =>
            ValidateObject(validationContext).Concat(ValidateCollectionItems(validationContext));        

        private IEnumerable<ValidationResult> ValidateObject(ValidationContext validationContext)
        {
            if (IsNotValid(validationContext, out var results))
            {
                return
                    from result in results
                    select new ValidationResult(result.ErrorMessage, result.MemberNames.Select(ApplyNameTo).ToArray());
            }
            return Enumerable.Empty<ValidationResult>();
        }

        private string ApplyNameTo(string childMemberName) =>
            $"{Name}.{childMemberName}";

        private IEnumerable<ValidationResult> ValidateCollectionItems(ValidationContext validationContext) =>
            from collection in ChildMemberCollection.FromChildMember(this)
            from result in collection.Validate(validationContext)
            select result;        

        #endregion        

        #region [====== ValidateChildMembers ======]

        private static readonly ConcurrentDictionary<Type, ChildMember[]> _ChildMembersPerType = new ConcurrentDictionary<Type, ChildMember[]>();
        private const BindingFlags _ChildMemberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        internal static IEnumerable<ValidationResult> ValidateChildMembers(ValidationContext validationContext) =>
            from childMember in _ChildMembersPerType.GetOrAdd(validationContext.ObjectType, GetChildMembersOfType)
            from result in childMember.Validate(validationContext)
            select result;

        private static ChildMember[] GetChildMembersOfType(Type type) =>
            GetChildMemberFieldsOfType(type).Concat(GetChildMemberPropertiesOfType(type)).ToArray();

        private static IEnumerable<ChildMember> GetChildMemberFieldsOfType(Type type) =>
            from field in type.GetFields(_ChildMemberBindingFlags)
            let attribute = field.GetCustomAttribute<ChildMemberAttribute>(true)
            where attribute != null
            select new ChildMemberField(field, attribute);

        private static IEnumerable<ChildMember> GetChildMemberPropertiesOfType(Type type) =>
            from property in type.GetProperties(_ChildMemberBindingFlags)
            let attribute = property.GetCustomAttribute<ChildMemberAttribute>(true)
            where attribute != null
            select new ChildMemberProperty(property, attribute);

        #endregion        
    }
}

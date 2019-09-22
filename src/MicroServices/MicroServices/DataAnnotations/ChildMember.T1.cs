using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Kingo.MicroServices.DataAnnotations
{
    internal abstract class ChildMember<TMemberInfo> : ChildMember where TMemberInfo : MemberInfo
    {        
        private readonly TMemberInfo _member;
        private readonly ChildMemberAttribute _attribute;        

        protected ChildMember(TMemberInfo member, ChildMemberAttribute attribute)
        {            
            _member = member;
            _attribute = attribute;               
        }        

        public override string Name =>
            _member.Name;

        protected TMemberInfo Member =>
            _member;

        protected override IEnumerable<ValidationResult> ValidateChildMember(ValidationContext validationContext)
        {
            var validationErrors = base.ValidateChildMember(validationContext).ToArray();
            if (validationErrors.Length > 0)
            {
                if (_attribute.TryGetValidationError(validationContext, out var result))
                {
                    yield return result;
                }
                foreach (var validationError in validationErrors)
                {
                    yield return validationError;
                }
            }
        }
    }
}

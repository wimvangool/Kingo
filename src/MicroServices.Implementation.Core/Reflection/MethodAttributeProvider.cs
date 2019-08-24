using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingo.Reflection
{
    internal sealed class MethodAttributeProvider : IMethodAttributeProvider
    {
        private readonly MemberAttributeProvider<MethodInfo> _attributeProvider;

        public MethodAttributeProvider(MethodInfo info)
        {
            _attributeProvider = new MemberAttributeProvider<MethodInfo>(info);
        }

        public MethodInfo Info =>
            _attributeProvider.Member;

        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();

        public override string ToString() =>
            ToString(Info.Name, Info.GetParameters().Select(parameter => parameter.ParameterType));

        private static string ToString(string methodName, IEnumerable<Type> parameterTypes) =>
            $"{methodName}({string.Join(", ", parameterTypes.Select(type => type.FriendlyName()))})";
    }
}

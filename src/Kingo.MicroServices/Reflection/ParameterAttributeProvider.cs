using System;
using System.Linq;
using System.Reflection;

namespace Kingo.Reflection
{
    internal sealed class ParameterAttributeProvider : AttributeProvider, IParameterAttributeProvider
    {
        private readonly ParameterInfo _parameter;
        
        public ParameterAttributeProvider(ParameterInfo parameter)
        {
            _parameter = parameter;
        }

        protected override object Target =>
            _parameter;

        protected override string TargetName =>
            _parameter.Name;

        public Type Type =>
            _parameter.ParameterType;

        public ParameterInfo Info =>
            _parameter;

        protected override Attribute[] LoadAttributes() =>
            _parameter.GetCustomAttributes(true).OfType<Attribute>().ToArray();
    }
}

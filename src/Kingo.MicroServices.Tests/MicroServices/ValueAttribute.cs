using System;

namespace Kingo.MicroServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    internal sealed class ValueAttribute : Attribute
    {
        public ValueAttribute(int value)
        {
            Value = value;
        }

        public int Value
        {
            get;
        }
    }
}

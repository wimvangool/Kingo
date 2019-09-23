using System.Reflection;
using Kingo.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class AsyncMethodTest
    {
        protected static void AssertComponentProperties<TComponent>(IAsyncMethod method, int value)
        {
            Assert.AreSame(typeof(TComponent), method.ComponentType);
            AssertValue(method.ComponentType, value);
        }

        protected static void AssertValue(MemberInfo member, int value)
        {
            Assert.IsTrue(member.TryGetAttributeOfType(out ValueAttribute attribute));
            Assert.AreEqual(value, attribute.Value);
        }

        protected static void AssertValue(ParameterInfo parameter, int value)
        {
            Assert.IsTrue(parameter.TryGetAttributeOfType(out ValueAttribute attribute));
            Assert.AreEqual(value, attribute.Value);
        }
    }
}

using Kingo.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class AsyncMethodTest
    {
        protected static void AssertComponentProperties<TComponent>(IAsyncMethod method, int value)
        {
            Assert.AreSame(typeof(TComponent), method.Component.Type);
            AssertValue(method.Component, value);
        }

        protected static void AssertValue(IAttributeProvider provider, int value)
        {
            Assert.IsTrue(provider.TryGetAttributeOfType(out ValueAttribute attribute));
            Assert.AreEqual(value, attribute.Value);
        }
    }
}

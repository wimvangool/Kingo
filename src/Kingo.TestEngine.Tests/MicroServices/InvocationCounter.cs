using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class InvocationCounter
    {
        private int _invocationCount;

        public int Increment() =>
            Interlocked.Increment(ref _invocationCount);

        public void AssertAtLeast(int minimumInvocationCount) =>
            Assert.IsTrue(minimumInvocationCount <= _invocationCount, $"Expected minimum invocation-count: {minimumInvocationCount}, but was {_invocationCount}");

        public void AssertExactly(int invocationCount) =>
            Assert.AreEqual(invocationCount, _invocationCount, $"Expected invocation-count: {invocationCount}, but was {_invocationCount}");
    }
}

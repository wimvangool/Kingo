using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class DisposableStub : Disposable
    {
        public void AssertHasBeenDisposed() =>
            Assert.IsTrue(IsDisposed, "Instance was not disposed.");

        public void AssertHasNotBeenDisposed() =>
            Assert.IsFalse(IsDisposed, "Instance was disposed.");
    }
}

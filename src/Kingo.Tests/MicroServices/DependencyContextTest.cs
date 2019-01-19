using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class DependencyContextTest
    {
        #region [====== Current ======]

        [TestMethod]
        public void Current_IsNull_IfNoScopeHasBeenCreated()
        {
            Assert.IsNull(DependencyContext.Current);
        }

        [TestMethod]
        public void Current_IsNotNull_IfScopeHasBeenCreated()
        {
            using (DependencyContext.CreateScope())
            {
                Assert.IsNotNull(DependencyContext.Current);
            }
        }

        [TestMethod]
        public void Current_DoesNotReferToSameInstanceAsOuterScope_IfScopesAreNested()
        {
            using (DependencyContext.CreateScope())
            {
                var context = DependencyContext.Current;

                using (DependencyContext.CreateScope())
                {
                    Assert.AreNotSame(context, DependencyContext.Current);
                }
                Assert.AreSame(context, DependencyContext.Current);
            }
        }

        #endregion

        #region [====== GetValue ======]

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetValue_Throws_IfContextIsRootContext_And_ContextHasBeenDisposed()
        {
            using (DependencyContext.CreateScope())
            {
                DependencyContext.Current.Dispose();
                DependencyContext.Current.GetValue(Guid.Empty);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetValue_Throws_IfContextIsChildContext_And_ContextHasBeenDisposed()
        {
            using (DependencyContext.CreateScope())
            {
                using (DependencyContext.CreateScope())
                {
                    DependencyContext.Current.Dispose();
                    DependencyContext.Current.GetValue(Guid.Empty);
                }
            }
        }

        [TestMethod]
        public void GetValue_ReturnsNull_IfContextIsRootContext_And_ValueIsNotSet()
        {
            using (DependencyContext.CreateScope())
            {
                Assert.IsNull(DependencyContext.Current.GetValue(Guid.Empty));
            }
        }

        [TestMethod]
        public void GetValue_ReturnsNull_IfContextIsChildContext_And_ValueIsNotSet()
        {
            using (DependencyContext.CreateScope())
            {
                using (DependencyContext.CreateScope())
                {
                    Assert.IsNull(DependencyContext.Current.GetValue(Guid.Empty));
                }                    
            }
        }

        [TestMethod]
        public void GetValue_ReturnsValue_IfContextIsRootContext_And_ValueIsSetInsideRootContext()
        {
            using (DependencyContext.CreateScope())
            {
                var key = Guid.NewGuid();
                var value = new object();

                DependencyContext.Current.SetValue(key, value);

                Assert.AreSame(value, DependencyContext.Current.GetValue(key));
            }
        }

        [TestMethod]
        public void GetValue_ReturnsValue_IfContextIsChildContext_And_ValueIsSetInsideRootContext()
        {
            using (DependencyContext.CreateScope())
            {
                var key = Guid.NewGuid();
                var value = new object();

                DependencyContext.Current.SetValue(key, value);

                using (DependencyContext.CreateScope())
                {
                    Assert.AreSame(value, DependencyContext.Current.GetValue(key));
                }                    
            }
        }

        [TestMethod]
        public void GetValue_ReturnsValue_IfContextIsRootContext_And_ValueIsSetInsideChildContext()
        {
            using (DependencyContext.CreateScope())
            {
                var key = Guid.NewGuid();
                var value = new object();                

                using (DependencyContext.CreateScope())
                {
                    DependencyContext.Current.SetValue(key, value);                    
                }

                Assert.AreSame(value, DependencyContext.Current.GetValue(key));
            }
        }

        [TestMethod]
        public void GetValue_ReturnsValue_IfValueIsSetMoreThanOnce()
        {
            using (DependencyContext.CreateScope())
            {
                var key = Guid.NewGuid();
                var value = new object();

                DependencyContext.Current.SetValue(key, value);
                DependencyContext.Current.SetValue(key, value);

                Assert.AreSame(value, DependencyContext.Current.GetValue(key));
            }
        }

        #endregion

        #region [====== SetValue ======]

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetValue_Throws_IfContextIsRootContext_And_ContextHasBeenDisposed()
        {
            using (DependencyContext.CreateScope())
            {
                DependencyContext.Current.Dispose();
                DependencyContext.Current.SetValue(Guid.Empty, null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetValue_Throws_IfContextIsChildContext_And_ContextHasBeenDisposed()
        {
            using (DependencyContext.CreateScope())
            {
                using (DependencyContext.CreateScope())
                {
                    DependencyContext.Current.Dispose();
                    DependencyContext.Current.SetValue(Guid.Empty, null);
                }
            }
        }

        [TestMethod]
        public void SetValue_AllowsNullValues()
        {
            using (DependencyContext.CreateScope())
            {
                var key = Guid.NewGuid();

                DependencyContext.Current.SetValue(key, null);

                Assert.IsNull(DependencyContext.Current.GetValue(key));
            }
        }

        [TestMethod]
        public void SetValue_RemovesStoredValue_IfAnotherValueWithTheSameKeyIsSet()
        {
            using (DependencyContext.CreateScope())
            {
                var key = Guid.NewGuid();
                var valueA = new DisposableStub();
                var valueB = new DisposableStub();

                DependencyContext.Current.SetValue(key, valueA);
                DependencyContext.Current.SetValue(key, valueB);

                valueA.AssertHasBeenDisposed();
                valueB.AssertHasNotBeenDisposed();
            }
        }

        #endregion

        #region [====== RemoveValue ======]

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveValue_Throws_IfContextIsRootContext_And_ContextHasBeenDisposed()
        {
            using (DependencyContext.CreateScope())
            {
                DependencyContext.Current.Dispose();
                DependencyContext.Current.RemoveValue(Guid.Empty);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveValue_Throws_IfContextIsChildContext_And_ContextHasBeenDisposed()
        {
            using (DependencyContext.CreateScope())
            {
                using (DependencyContext.CreateScope())
                {
                    DependencyContext.Current.Dispose();
                    DependencyContext.Current.RemoveValue(Guid.Empty);
                }
            }
        }

        [TestMethod]
        public void RemoveValue_ReturnsFalse_IfValueWasNotStored()
        {
            using (DependencyContext.CreateScope())
            {
                Assert.IsFalse(DependencyContext.Current.RemoveValue(Guid.Empty));
            }
        }

        [TestMethod]
        public void RemoveValue_ReturnsTrueAndDisposesValue_IfValueWasStored()
        {
            using (DependencyContext.CreateScope())
            {
                var key = Guid.NewGuid();
                var value = new DisposableStub();

                DependencyContext.Current.SetValue(key, value);

                Assert.IsTrue(DependencyContext.Current.RemoveValue(key));

                value.AssertHasBeenDisposed();
            }
        }

        #endregion

        #region [====== Dispose ======]

        [TestMethod]
        public void Dispose_DisposesAllDisposableValues_IfContextIsRootContext()
        {
            (Guid key, DisposableStub value) a = (Guid.NewGuid(), new DisposableStub());
            (Guid key, object value) b = (Guid.NewGuid(), new object());
            (Guid key, object value) c = (Guid.NewGuid(), null);

            using (DependencyContext.CreateScope())
            {
                DependencyContext.Current.SetValue(a.key, a.value);
                DependencyContext.Current.SetValue(b.key, b.value);
                DependencyContext.Current.SetValue(c.key, c.value);
            }

            a.value.AssertHasBeenDisposed();
        }

        [TestMethod]
        public void Dispose_DoesNotDisposesAllDisposableValues_IfContextIsChildContext()
        {
            using (DependencyContext.CreateScope())
            {
                (Guid key, DisposableStub value) a = (Guid.NewGuid(), new DisposableStub());
                (Guid key, object value) b = (Guid.NewGuid(), new object());
                (Guid key, object value) c = (Guid.NewGuid(), null);

                using (DependencyContext.CreateScope())
                {
                    DependencyContext.Current.SetValue(a.key, a.value);
                    DependencyContext.Current.SetValue(b.key, b.value);
                    DependencyContext.Current.SetValue(c.key, c.value);
                }

                a.value.AssertHasNotBeenDisposed();
            }                
        }

        #endregion
    }
}

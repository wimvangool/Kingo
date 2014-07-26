using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Server
{
    [TestClass]
    public sealed class PerUnitOfWorkLifetimeManagerTest
    {
        #region [====== Setup and Teardown ======]

        private CacheBasedLifetimeManager _lifetimeManager;

        [TestInitialize]
        public void Setup()
        {
            _lifetimeManager = new CacheBasedLifetimeManager(UnitOfWorkContext.Cache);
        }

        #endregion

        #region [====== Tests ======]

        [TestMethod]
        public void GetValue_ReturnsNull_IfCalledOutsideContextScope()
        {
            Assert.IsNull(_lifetimeManager.GetValue());
        }

        [TestMethod]
        public void GetValue_ReturnsNull_IfNoValueHasBeenStored()
        {
            Processor.Process(new object(), message =>            
                Assert.IsNull(_lifetimeManager.GetValue())             
            );
        }

        [TestMethod]
        public void GetValue_ReturnsStoredValue_IfValueWasStoredInCache()
        {
            var instance = new object();

            Processor.Process(instance, message =>
            {
                _lifetimeManager.SetValue(instance);

                Assert.AreSame(instance, _lifetimeManager.GetValue());                
            });
            Assert.IsNull(_lifetimeManager.GetValue());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveValue_Throws_IfCalledOutsideContextScope()
        {
            _lifetimeManager.SetValue(new object());
        }

        [TestMethod]
        public void RemoveValue_RemovesValueFromTheCache_IfValueWasStoredInCache()
        {
            var instance = new object();

            Processor.Process(instance, message =>
            {
                _lifetimeManager.SetValue(instance);
                _lifetimeManager.RemoveValue();

                Assert.IsNull(_lifetimeManager.GetValue());                
            });
            Assert.IsNull(_lifetimeManager.GetValue());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetValue_Throws_IfCalledOutsideContextScope()
        {
            _lifetimeManager.SetValue(new object());
        }

        #endregion

        private static UnityTestProcessor Processor
        {
            get { return UnityTestProcessor.Instance; }
        }
    }
}

using System;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ComponentModel.Server
{
    [TestClass]
    public sealed class PerUnitOfWorkLifetimeManagerTest
    {
        #region [====== Setup and Teardown ======]

        private UnityTestProcessor _processor;
        private CacheBasedLifetimeManager _lifetimeManager;

        [TestInitialize]
        public void Setup()
        {
            _processor = new UnityTestProcessor();
            _lifetimeManager = new CacheBasedLifetimeManager(new UnitOfWorkCache());
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
            _processor.Handle(new EmptyMessage(), message =>            
                Assert.IsNull(_lifetimeManager.GetValue())             
            );
        }

        [TestMethod]
        public void GetValue_ReturnsStoredValue_IfValueWasStoredInCache()
        {
            var instance = new EmptyMessage();

            _processor.Handle(instance, message =>
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
            var instance = new EmptyMessage();

            _processor.Handle(instance, message =>
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
    }
}

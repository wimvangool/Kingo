using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kingo.ComponentModel.Server
{
    [TestClass]
    public sealed class PerUnitOfWorkLifetimeManagerTest
    {
        #region [====== Setup and Teardown ======]

        private UnityTestProcessor _processor;
        private PerUnitOfWorkLifetimeManager _lifetimeManager;

        [TestInitialize]
        public void Setup()
        {
            _processor = new UnityTestProcessor();
            _lifetimeManager = new PerUnitOfWorkLifetimeManager();
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
        public void RemoveValue_ImmediatelyDisposesValue_IfCalledOutsideContextScope()
        {
            var valueMock = new Mock<IDisposable>();
            valueMock.Setup(value => value.Dispose());

            _lifetimeManager.SetValue(valueMock.Object);

            valueMock.VerifyAll();
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

        #endregion        
    }
}

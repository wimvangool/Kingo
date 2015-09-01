using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceComponents.ComponentModel.Server
{
    [TestClass]
    public sealed class MessageToStrategyMappingTest
    {
        private MessageToStrategyMapping<object> _mapping;

        [TestInitialize]
        public void Setup()
        {
            _mapping = new MessageToStrategyMapping<object>();
        }

        #region [====== Add ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfMessageIsNull()
        {
            _mapping.Add(null as IMessage, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfMessageTypeIsNull()
        {
            _mapping.Add(null as Type, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfMessageTypeIdIsNull()
        {
            _mapping.Add(null as string, new object());
        }   
     
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfMappingForSpecifiedMessageAlreadyExists()
        {
            var messageTypeId = Guid.NewGuid().ToString();

            _mapping.Add(messageTypeId, new object());
            _mapping.Add(messageTypeId, new object());
        }        

        #endregion       

        #region [====== TryGetStrategy ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetStrategy_Throws_IfMessageIsNull()
        {
            IMessage message = null;
            object strategy;

            _mapping.TryGetStrategy(message, out strategy);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetStrategy_Throws_IfMessageTypeIsNull()
        {
            Type messageType = null;
            object strategy;

            _mapping.TryGetStrategy(messageType, out strategy);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetStrategy_Throws_IfMessageTypeIdIsNull()
        {
            string messageTypeId = null;
            object strategy;

            _mapping.TryGetStrategy(messageTypeId, out strategy);
        }

        [TestMethod]
        public void TryGetStrategy_ReturnsFalse_IfMappingIsNotFound()
        {
            var messageTypeId = Guid.NewGuid().ToString();
            object strategy;

            Assert.IsFalse(_mapping.TryGetStrategy(messageTypeId, out strategy));
            Assert.IsNull(strategy);
        }        

        [TestMethod]
        public void TryGetStrategy_ReturnsTrue_IfMappingIsFound()
        {
            var messageTypeId = Guid.NewGuid().ToString();
            object addedStrategy = new object();
            object retrievedStrategy;

            _mapping.Add(messageTypeId, addedStrategy);

            Assert.IsTrue(_mapping.TryGetStrategy(messageTypeId, out retrievedStrategy));
            Assert.AreSame(addedStrategy, retrievedStrategy);
        }        

        #endregion
    }
}

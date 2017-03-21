using System;
using Kingo.Messaging.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
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

        #endregion
    }
}

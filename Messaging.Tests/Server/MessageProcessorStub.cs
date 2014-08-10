using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Server
{
    internal sealed class MessageProcessorStub : MessageProcessor
    {
        private readonly MessageHandlerFactory _factory;

        public MessageProcessorStub()
        {
            _factory = new MessageHandlerFactoryForUnity();
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _factory; }
        }
    }
}

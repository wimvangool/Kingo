﻿
namespace System.ComponentModel.Messaging.Server.SampleHandlers.ForTryRegisterInTests
{    
    internal sealed class GenericCommandHandler<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        public void Handle(TMessage message) {}
    }
}
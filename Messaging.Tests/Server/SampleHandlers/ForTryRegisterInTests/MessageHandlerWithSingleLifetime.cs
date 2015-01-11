﻿
namespace System.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler(InstanceLifetime.Singleton)]
    internal sealed class MessageHandlerWithSingleLifetime : IMessageHandler<Command>
    {        
        public void Handle(Command message) {}  
    }
}

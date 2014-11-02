﻿
namespace System.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
{
    [InstanceLifetime(InstanceLifetime.PerUnitOfWork)]
    internal sealed class MessageHandlerWithPerUnitOfWorkLifetime : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}

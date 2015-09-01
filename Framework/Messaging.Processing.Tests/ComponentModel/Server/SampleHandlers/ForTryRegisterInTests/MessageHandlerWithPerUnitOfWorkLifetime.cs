﻿
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler(InstanceLifetime.PerUnitOfWork)]
    internal sealed class MessageHandlerWithPerUnitOfWorkLifetime : IMessageHandler<Command>
    {
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }
    }
}

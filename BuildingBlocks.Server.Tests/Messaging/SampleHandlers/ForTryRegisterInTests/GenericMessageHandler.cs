﻿using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging.SampleHandlers.ForTryRegisterInTests
{    
    internal sealed class GenericCommandHandler<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        public Task HandleAsync(TMessage message)
        {
            return Task.Delay(0);
        }
    }
}
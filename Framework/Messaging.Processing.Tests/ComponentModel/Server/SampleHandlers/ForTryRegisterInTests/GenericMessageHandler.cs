﻿
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
{    
    internal sealed class GenericCommandHandler<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        public Task HandleAsync(TMessage message)
        {
            return Task.Delay(0);
        }
    }
}

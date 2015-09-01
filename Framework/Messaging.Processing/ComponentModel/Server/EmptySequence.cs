﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceComponents.Threading;

namespace ServiceComponents.ComponentModel.Server
{
    internal sealed class EmptyMessageSequence : MessageSequence
    {
        public override Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            return AsyncMethod.Void;
        }

        public override IMessageSequence Append(IMessageSequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }
            return sequence;
        }

        public override IMessageSequence Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            return new MessageSequenceNode<TMessage>(message, handler);
        }
    }
}

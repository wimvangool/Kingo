using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.ComponentModel.Messaging.Server
{
    internal sealed class MessageHandlerSpy : IMessageHandler<MessageOne>, IMessageHandler<MessageTwo>
    {
        private int _messageOneCount;
        private int _messageTwoCount;

        public int MessageOneCount
        {
            get { return _messageOneCount; }
        }

        public int MessageTwoCount
        {
            get { return _messageTwoCount; }
        }

        void IMessageHandler<MessageOne>.Handle(MessageOne message)
        {
            Interlocked.Increment(ref _messageOneCount);
        }

        void IMessageHandler<MessageTwo>.Handle(MessageTwo message)
        {
            Interlocked.Increment(ref _messageTwoCount);
        }
    }
}

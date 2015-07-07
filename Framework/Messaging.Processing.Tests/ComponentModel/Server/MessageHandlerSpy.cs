using System.Threading;
using System.Threading.Tasks;
using Syztem.Threading;

namespace Syztem.ComponentModel.Server
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

        Task IMessageHandler<MessageOne>.HandleAsync(MessageOne message)
        {
            return AsyncMethod.RunSynchronously(() =>
                 Interlocked.Increment(ref _messageOneCount)
            );            
        }

        Task IMessageHandler<MessageTwo>.HandleAsync(MessageTwo message)
        {
            return AsyncMethod.RunSynchronously(() =>
                 Interlocked.Increment(ref _messageTwoCount)
            );             
        }
    }
}

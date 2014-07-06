using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    public class MessageProcessorCommand : Command
    {
        private readonly IMessageProcessor _processor;

        public MessageProcessorCommand(IMessageProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            _processor = processor;
        }

        protected IMessageProcessor Processor
        {
            get {  return _processor; }
        }

        protected override void Execute(CancellationToken? token)
        {
            throw new NotImplementedException();
        }
    }
}

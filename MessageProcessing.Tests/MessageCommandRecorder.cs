using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageCommandRecorder
    {
        private readonly List<IMessageCommand> _commands;

        public MessageCommandRecorder()
        {
            _commands = new List<IMessageCommand>();
        }

        public void Record<TMessage>(IExternalMessageHandler<TMessage> handler, TMessage message) where TMessage : class
        {
            Record(new ExternalMessageHandler<TMessage>(handler) as IMessageHandler<TMessage>, message);
        }

        public void Record<TMessage>(IInternalMessageHandler<TMessage> handler, TMessage message) where TMessage : class
        {
            Record(new InternalMessageHandler<TMessage>(handler) as IMessageHandler<TMessage>, message);
        }

        public void Record<TMessage>(IMessageHandler<TMessage> handler, TMessage message) where TMessage : class
        {
            _commands.Add(new MessageCommand<TMessage>(handler, message));
        }        

        public int RecordCount(Type handlerType, object message)
        {
            return _commands.Count(command => command.Handler.GetType() == handlerType && command.Message == message);
        }

        private static readonly ThreadLocal<MessageCommandRecorder> _Current = new ThreadLocal<MessageCommandRecorder>();

        public static MessageCommandRecorder Current
        {
            get { return _Current.Value; }
            set { _Current.Value = value; }
        }
    }
}

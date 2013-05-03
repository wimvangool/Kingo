using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class ActionHandler<TMessage> : IExternalMessageHandler<TMessage> where TMessage : class
    {
        private readonly Action<TMessage> _action;

        public ActionHandler(Action<TMessage> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
        }

        public void Handle(TMessage message)
        {
            _action.Invoke(message);
        }
    }
}

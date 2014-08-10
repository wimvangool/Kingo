namespace System.ComponentModel.Messaging.Client
{
    internal sealed class MessageStateTracker
    {
        private readonly IMessage _message;
        private Guid _requestId;
        private bool _messageHadChanges;

        public MessageStateTracker(IMessage message)
        {
            _message = message;
        }

        public void NotifyExecutionStarted(Guid requestId)
        {
            _requestId = requestId;
            _messageHadChanges = _message.HasChanges;
        }

        public void NotifyExecutionEndedPrematurely(Guid requestId)
        {
            if (_requestId.Equals(requestId))
            {
                _message.HasChanges = _message.HasChanges || _messageHadChanges;
                _requestId = Guid.Empty;
            }
        }
    }
}

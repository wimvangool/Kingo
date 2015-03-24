namespace System.ComponentModel.Client
{
    internal sealed class RequestMessage : IMessage<RequestMessage>, IEquatable<RequestMessage>
    {
        private readonly IRequestMessageDispatcher _dispatcher;

        internal RequestMessage(IRequestMessageDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public string TypeId
        {
            get { return _dispatcher.MessageTypeId; }
        }

        #region [====== Copy ======]

        IMessage IMessage.Copy()
        {
            return Copy();
        }

        public RequestMessage Copy()
        {
            return new RequestMessage(_dispatcher);
        }

        #endregion

        #region [====== Equality ======]

        public override bool Equals(object obj)
        {
            return Equals(obj as RequestMessage);
        }

        public bool Equals(RequestMessage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Equals(_dispatcher, other._dispatcher);
        }

        public override int GetHashCode()
        {
            return _dispatcher.GetType().GetHashCode();
        }

        #endregion

        #region [====== Validation ======]        

        public ValidationErrorTree Validate()
        {
            return ValidationErrorTree.NoErrors;
        }

        #endregion
    }
}

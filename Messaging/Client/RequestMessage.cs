namespace System.ComponentModel.Client
{
    internal sealed class RequestMessage : Message<RequestMessage>
    {
        private readonly IRequestMessageDispatcher _dispatcher;

        internal RequestMessage(IRequestMessageDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RequestMessage);
        }

        private bool Equals(RequestMessage other)
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

        public override RequestMessage Copy()
        {
            return new RequestMessage(_dispatcher);
        }
    }
}

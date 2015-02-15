namespace System.ComponentModel.Client
{
    internal sealed class RequestMessage : Message<RequestMessage>
    {
        private readonly Type _returnType;

        internal RequestMessage(Type returnType)
        {
            _returnType = returnType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }
            var other = obj as RequestMessage;

            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return _returnType == other._returnType;
        }

        public override int GetHashCode()
        {
            return _returnType.GetHashCode();
        }

        public override RequestMessage Copy()
        {
            return new RequestMessage(_returnType);
        }
    }
}

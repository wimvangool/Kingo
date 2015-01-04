namespace System.ComponentModel.Server
{
    internal sealed class FuncDecorator<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut>
        where TMessageIn : class, IRequestMessage<TMessageIn>
        where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly Func<TMessageIn, TMessageOut> _query;

        internal FuncDecorator(Func<TMessageIn, TMessageOut> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            _query = query;
        }

        public TMessageOut Execute(TMessageIn message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return _query.Invoke(message);
        }
    }
}

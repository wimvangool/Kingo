namespace System.ComponentModel.Server
{
    internal sealed class Query<TMessageOut> : IQuery<TMessageOut> where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly IQuery<TMessageOut> _nextQuery;
        private readonly QueryModule _nextModule;

        internal Query(IQuery<TMessageOut> nextQuery, QueryModule nextModule)
        {
            _nextQuery = nextQuery;
            _nextModule = nextModule;
        }

        public IMessage MessageIn
        {
            get { return _nextQuery.MessageIn; }
        }        

        public TMessageOut Invoke()
        {
            return _nextModule.Invoke(_nextQuery);
        }        
    }
}

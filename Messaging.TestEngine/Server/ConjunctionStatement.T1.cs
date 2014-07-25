namespace System.ComponentModel.Messaging.Server
{
    internal sealed class ConjunctionStatement<TValue> : IConjunctionStatement<TValue>
    {
        private readonly TValue _value;
        private readonly bool _statementShouldBeInvoked;

        public ConjunctionStatement(TValue value, bool statementShouldBeInvoked)
        {
            _value = value;
            _statementShouldBeInvoked = statementShouldBeInvoked;
        }        

        public void And(Action<TValue> statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            if (_statementShouldBeInvoked)
            {
                statement.Invoke(_value);    
            }
        }        
    }
}

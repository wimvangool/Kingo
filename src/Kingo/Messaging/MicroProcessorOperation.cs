namespace Kingo.Messaging
{
    /// <summary>
    /// Represents an operation that is being performed by a <see cref="MicroProcessor" />.
    /// </summary>
    public sealed class MicroProcessorOperation
    {        
        internal MicroProcessorOperation(MicroProcessorOperationTypes type, object message)
        {                        
            Type = type;
            Message = message;
        }        

        /// <summary>
        /// Returns the type of this operation.
        /// </summary>
        public MicroProcessorOperationTypes Type
        {
            get;
        }

        /// <summary>
        /// Returns the message associated with this operation.
        /// </summary>
        public object Message
        {
            get;
        }        

        /// <inheritdoc />
        public override string ToString() =>
            $"[{Type}] {Message}";        
    }
}

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
            MessageAttributeProvider = Message == null ? TypeAttributeProvider.None : new TypeAttributeProvider(message.GetType());
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
        
        /// <summary>
        /// Returns the attribute-provider for the <see cref="Message"/> of this operation.
        /// </summary>
        public ITypeAttributeProvider MessageAttributeProvider
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"[{Type}] {Message}";        
    }
}

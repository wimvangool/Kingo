using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel.Server
{
    internal sealed class ReadOnlyMappingState<TStrategy> : MessageToStrategyMappingState<TStrategy> where TStrategy : class
    {
        private readonly Dictionary<string, TStrategy> _mapping;

        internal ReadOnlyMappingState(Dictionary<string, TStrategy> mapping)
        {
            _mapping = mapping;
        }

        protected override Dictionary<string, TStrategy> Mapping
        {
            get { return _mapping; }
        }

        internal override void Add(string messageTypeId, TStrategy strategy)
        {
            throw NewMappingIsReadOnlyException();
        }

        internal override void SwitchToReadOnly(ref MessageToStrategyMappingState<TStrategy> currentState)
        {
            Interlocked.Exchange(ref currentState, this);
        }

        internal override bool TryGetStrategy(string messageTypeId, out TStrategy strategy)
        {
            return Mapping.TryGetValue(messageTypeId, out strategy);
        }

        private static Exception NewMappingIsReadOnlyException()
        {
            throw new InvalidOperationException(ExceptionMessages.MessageToStrategyMapping_IsReadOnly);
        }        
    }
}

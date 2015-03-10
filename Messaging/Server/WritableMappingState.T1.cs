using System.Collections.Generic;
using System.Threading;

namespace System.ComponentModel.Server
{
    internal sealed class WritableMappingState<TStrategy> : MessageToStrategyMappingState<TStrategy> where TStrategy : class
    {
        private readonly Dictionary<string, TStrategy> _mapping;

        internal WritableMappingState()
        {
            _mapping = new Dictionary<string, TStrategy>();
        }

        protected override Dictionary<string, TStrategy> Mapping
        {
            get { return _mapping; }
        }

        internal override void Add(string messageTypeId, TStrategy strategy)
        {
            Mapping.Add(messageTypeId, strategy);
        }

        internal override void SwitchToReadOnly(ref MessageToStrategyMappingState<TStrategy> currentState)
        {
            Interlocked.CompareExchange(ref currentState, new ReadOnlyMappingState<TStrategy>(Mapping), this);
        }

        internal override bool TryGetStrategy(string messageTypeId, out TStrategy strategy)
        {
            return Mapping.TryGetValue(messageTypeId, out strategy);
        }
    }
}

using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal abstract class MessageToStrategyMappingState<TStrategy> where TStrategy : class
    {
        protected abstract Dictionary<string, TStrategy> Mapping
        {
            get;
        }

        internal abstract void Add(string messageTypeId, TStrategy strategy);

        internal abstract void SwitchToReadOnly(ref MessageToStrategyMappingState<TStrategy> currentState);        

        internal abstract bool TryGetStrategy(string messageTypeId, out TStrategy strategy);
    }
}

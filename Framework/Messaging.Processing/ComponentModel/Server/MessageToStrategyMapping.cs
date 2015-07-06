using System.Collections.Concurrent;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for every <see cref="MessageToStrategyMapping{T}" /> instance.
    /// </summary>
    public abstract class MessageToStrategyMapping
    {
        internal MessageToStrategyMapping() { }

        private static readonly ConcurrentDictionary<Type, object> _Mappings = new ConcurrentDictionary<Type, object>();

        internal static IMessageToStrategyMapping<TStrategy> GetOrAdd<TStrategy>(Type moduleType, Func<IMessageToStrategyMapping<TStrategy>> mappingFactory)
            where TStrategy : class
        {                       
            return (IMessageToStrategyMapping<TStrategy>) _Mappings.GetOrAdd(moduleType, _ => mappingFactory.Invoke());
        }
    }
}

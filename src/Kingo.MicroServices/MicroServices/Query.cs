using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represent a component that implements one or more variations of the <see cref="IQuery{TResponse}"/> or
    /// <see cref="IQuery{TRequest, TResponse}"/> interfaces.
    /// </summary>
    public abstract class Query : MicroProcessorComponent
    {
        private readonly QueryInterface[] _interfaces;

        internal Query(MicroProcessorComponent component, params QueryInterface[] interfaces) :
            base(component)
        {
            _interfaces = interfaces;
        }

        /// <summary>
        /// Returns the <see cref="IQuery{TResponse}"/> ans <see cref="IQuery{TRequest, TResponse}"/> interfaces
        /// that are implemented by this query.
        /// </summary>
        public IReadOnlyCollection<QueryInterface> Interfaces =>
            _interfaces;

        /// <inheritdoc />
        public override string ToString() =>
            $"{Type.FriendlyName()} ({_interfaces.Length} interface(s) implemented)";
    }
}

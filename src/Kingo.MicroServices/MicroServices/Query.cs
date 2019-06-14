using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represent a component that implements one or more variations of the <see cref="IQuery{TResponse}"/> or
    /// <see cref="IQuery{TRequest, TResponse}"/> interfaces.
    /// </summary>
    public abstract class Query : MicroProcessorComponent, IReadOnlyCollection<ExecuteAsyncMethod>
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

        int IReadOnlyCollection<ExecuteAsyncMethod>.Count =>
            _interfaces.Length;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<ExecuteAsyncMethod> GetEnumerator() =>
            Methods().GetEnumerator();

        private IEnumerable<ExecuteAsyncMethod> Methods() =>
            _interfaces.Select(@interface => @interface.CreateMethod(this));

        /// <inheritdoc />
        public override string ToString() =>
            $"{Type.FriendlyName()} ({MessageHandlerOrQueryInterface.ToString(_interfaces)}";
    }
}

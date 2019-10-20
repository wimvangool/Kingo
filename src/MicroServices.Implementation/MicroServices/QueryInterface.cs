using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represent a specific, closed version of the <see cref="IQuery{TResponse}"/> or
    /// <see cref="IQuery{TRequest, TResponse}"/> interface.
    /// </summary>
    public sealed class QueryInterface : MessageHandlerOrQueryInterface<QueryComponent, ExecuteAsyncMethod>
    {        
        private readonly Type[] _messageTypes;

        private QueryInterface(Type type) :
            base(type)
        {            
            _messageTypes = type.GetGenericArguments();
        }        

        /// <summary>
        /// The request message type. This returns <c>null</c> if this interface represents an instance of the
        /// <see cref="IQuery{TResponse}"/> interface.
        /// </summary>
        public Type RequestType =>
            HasRequestType ? _messageTypes[0] : null;

        /// <summary>
        /// The response message type.
        /// </summary>
        public Type ResponseType =>
            HasRequestType ? _messageTypes[1] : _messageTypes[0];        

        internal override string MethodName =>
            HasRequestType ? nameof(IQuery<object, object>.ExecuteAsync) : nameof(IQuery<object>.ExecuteAsync);

        private bool HasRequestType =>
            _messageTypes.Length == 2;

        internal override ExecuteAsyncMethod CreateMethod(QueryComponent component) =>
            new ExecuteAsyncMethod(component, this);

        #region [====== FromComponent ======]

        internal static IEnumerable<QueryInterface> FromComponent(MicroProcessorComponent component) =>
            from queryInterface in component.Type.GetInterfacesOfType(typeof(IQuery<>), typeof(IQuery<,>))
            select new QueryInterface(queryInterface);

        #endregion

        #region [====== FromType ======]

        internal static QueryInterface FromType<TResponse>() =>
            new QueryInterface(typeof(IQuery<TResponse>));

        internal static QueryInterface FromType<TRequest, TResponse>() =>
            new QueryInterface(typeof(IQuery<TRequest, TResponse>));

        #endregion
    }
}

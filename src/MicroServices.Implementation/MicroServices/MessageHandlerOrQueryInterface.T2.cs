using System;
using System.Collections.Generic;
using Kingo.MicroServices.Controllers;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents a specific variant of the <see cref="IMessageHandler{TMessage}"/>, <see cref="IQuery{TResponse}"/>
    /// or <see cref="IQuery{TRequest, TResponse}"/> interface.
    /// </summary>
    /// <typeparam name="TComponent">Type of the associated component.</typeparam>
    /// <typeparam name="TMethod">Type of the associated method.</typeparam>
    public abstract class MessageHandlerOrQueryInterface<TComponent, TMethod> : MicroProcessorComponentInterface
        where TComponent : MicroProcessorComponent
        where TMethod : IAsyncMethod
    {
        private readonly Type _implementingType;

        internal MessageHandlerOrQueryInterface(Type type, Type implementingType = null) :
            base(type)
        {
            _implementingType = implementingType ?? type;
        }

        /// <summary>
        /// Represents the interface that inherits the interface-type.
        /// </summary>
        public Type ImplementingType =>
            _implementingType;

        internal IEnumerable<Type> ServiceTypes =>
            new[] { Type, ImplementingType };

        internal abstract TMethod CreateMethod(TComponent component);
    }
}

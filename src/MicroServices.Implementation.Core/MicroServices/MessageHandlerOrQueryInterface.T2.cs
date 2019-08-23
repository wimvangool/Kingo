using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents a specific variant of the <see cref="IMessageHandler{TMessage}"/>, <see cref="IQuery{TResponse}"/>
    /// or <see cref="IQuery{TRequest, TResponse}"/> interface.
    /// </summary>
    /// <typeparam name="TComponent">Type of the associated component.</typeparam>
    /// <typeparam name="TMethod">Type of the associated method.</typeparam>
    public abstract class MessageHandlerOrQueryInterface<TComponent, TMethod> : MessageHandlerOrQueryInterface
        where TComponent : MicroProcessorComponent
        where TMethod : IAsyncMethod
    {        
        internal MessageHandlerOrQueryInterface(Type type) :
            base(type) { }

        internal abstract TMethod CreateMethod(TComponent component);
    }
}

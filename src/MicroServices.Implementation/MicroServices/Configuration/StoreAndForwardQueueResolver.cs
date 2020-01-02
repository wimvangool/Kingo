using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Kingo.MicroServices.Controllers;
using Kingo.Reflection;

namespace Kingo.MicroServices.Configuration
{
    internal sealed class StoreAndForwardQueueResolver : IMicroServiceBusResolver
    {
        private readonly ThreadLocal<bool> _isResolvingQueue;
        private readonly IMicroServiceBusResolver _nextResolver;
        private readonly Type _queueType;

        public StoreAndForwardQueueResolver(IMicroServiceBusResolver nextResolver, Type queueType)
        {
            _isResolvingQueue = new ThreadLocal<bool>();
            _nextResolver = nextResolver;
            _queueType = queueType;
        }

        private bool IsResolvingQueue
        {
            get => _isResolvingQueue.Value;
            set => _isResolvingQueue.Value = value;
        }

        // When resolving a StoreAndForwardQueue, we manually inspect its constructor. If
        // its constructor contains a IMicroServiceBus-dependency, which is to be expected, then
        // we inject an instance that is resolved by the next resolver, which represents the rest
        // of the pipeline. The other dependencies are simply resolved through the provided IServiceProvider.
        public IMicroServiceBus ResolveMicroServiceBus(IServiceProvider serviceProvider)
        {
            if (IsResolvingQueue)
            {
                throw NewCircularDependencyDetectedException(_queueType);
            }
            IsResolvingQueue = true;

            try
            {
                return ResolveMicroServiceBus(serviceProvider, FindPublicQueueConstructor()) as IMicroServiceBus;
            }
            finally
            {
                IsResolvingQueue = false;
            }
        }

        private object ResolveMicroServiceBus(IServiceProvider serviceProvider, ConstructorInfo constructor) =>
            constructor.Invoke(ResolveParameters(serviceProvider, constructor.GetParameters()).ToArray());

        private ConstructorInfo FindPublicQueueConstructor()
        {
            try
            {
                return _queueType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            }
            catch (InvalidOperationException)
            {
                throw NewCannotFindConstructorException(_queueType);
            }
        }

        private IEnumerable<object> ResolveParameters(IServiceProvider serviceProvider, IEnumerable<ParameterInfo> parameters)
        {
            IMicroServiceBus microServiceBus = null;

            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == typeof(IMicroServiceBus))
                {
                    if (microServiceBus == null)
                    {
                        microServiceBus = _nextResolver.ResolveMicroServiceBus(serviceProvider);
                    }
                    yield return microServiceBus;
                }
                else
                {
                    yield return serviceProvider.GetService(parameter.ParameterType);
                }
            }
        }

        public override string ToString() =>
            $"{_queueType.FriendlyName()} --> {_nextResolver}";

        private static Exception NewCircularDependencyDetectedException(Type queueType)
        {
            var messageFormat = ExceptionMessages.StoreAndForwardQueueResolver_CircularDependencyDetected;
            var message = string.Format(messageFormat, queueType.FriendlyName());
            return new InvalidOperationException(message);
        }

        private static Exception NewCannotFindConstructorException(Type queueType)
        {
            var messageFormat = ExceptionMessages.StoreAndForwardQueueResolver_CannotFindConstructor;
            var message = string.Format(messageFormat, queueType.FriendlyName());
            return new InvalidOperationException(message);
        }
    }
}

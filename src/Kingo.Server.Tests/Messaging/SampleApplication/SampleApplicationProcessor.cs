using System;
using System.Reflection;

namespace Kingo.Messaging.SampleApplication
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public sealed class SampleApplicationProcessor : MessageProcessor
    {                
        protected override MessageHandlerFactory CreateMessageHandlerFactory(LayerConfiguration layers)
        {
            var factory = new UnityFactory();

            factory.RegisterMessageHandlers(Assembly.GetExecutingAssembly().GetTypes(), IsHandlerForMessageProcessorTests);
            factory.RegisterDependencies(Assembly.GetExecutingAssembly().GetTypes(), null, IsRepositoryInterface, null);

            return factory;
        }

        private static bool IsHandlerForMessageProcessorTests(Type type)
        {
            return type.Namespace.EndsWith("SampleApplication");
        }

        private static bool IsRepositoryInterface(Type type)
        {
            return type.IsInterface && type.Name.EndsWith("Repository");
        }
    }
}
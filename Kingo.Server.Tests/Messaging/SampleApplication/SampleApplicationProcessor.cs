using System;
using System.Reflection;

namespace Kingo.Messaging.SampleApplication
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public sealed class SampleApplicationProcessor : MessageProcessor
    {                
        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();

            factory.RegisterMessageHandlers(Assembly.GetExecutingAssembly(), IsHandlerForMessageProcessorTests);
            factory.RegisterDependencies(Assembly.GetExecutingAssembly(), null, IsRepositoryInterface, null);

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
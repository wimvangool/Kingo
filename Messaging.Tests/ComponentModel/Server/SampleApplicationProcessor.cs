using System.Reflection;

namespace System.ComponentModel.Server
{
    internal sealed class SampleApplicationProcessor : MessageProcessor
    {                
        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();

            factory.RegisterMessageHandlers(Assembly.GetExecutingAssembly(), IsHandlerForMessageProcessorTests);
            factory.RegisterDependencies(Assembly.GetExecutingAssembly(), null, IsRepositoryInterface);

            return factory;
        }               

        private static bool IsHandlerForMessageProcessorTests(Type type)
        {
            return type.Namespace == "System.ComponentModel.Server.SampleApplication.MessageHandlers";
        }

        private static bool IsRepositoryInterface(Type type)
        {
            return type.IsInterface && type.Name.EndsWith("Repository");
        }
    }
}

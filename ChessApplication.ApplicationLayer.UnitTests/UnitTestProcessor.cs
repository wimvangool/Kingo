using System;
using System.ComponentModel.Server;
using System.Reflection;

namespace SummerBreeze.ChessApplication
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public sealed class UnitTestProcessor : MessageProcessor
    {
        private readonly UnityFactory _messageHandlerFactory;

        private UnitTestProcessor(UnityFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }

        private static readonly Lazy<UnitTestProcessor> _Instance = new Lazy<UnitTestProcessor>(CreateProcessor, true);

        /// <summary>
        /// Returns the instance of <see cref="UnitTestProcessor" />.
        /// </summary>
        public static UnitTestProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static UnitTestProcessor CreateProcessor()
        {
            return new UnitTestProcessor(CreateMessageHandlerFactory());
        }

        private static UnityFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();
            factory.RegisterMessageHandlers(FromApplicationLayer());
            return factory;
        }

        private static AssemblySet FromApplicationLayer()
        {
            return AssemblySet.FromCurrentDirectory("*ApplicationLayer.MessageHandlers.dll");
        }
    }
}
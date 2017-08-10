using System;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerFactoryStub : MessageHandlerFactory
    {
        public override string ToString() =>
            "<None>";

        #region [====== Read ======]

        protected internal override object Resolve(Type type) =>
            throw NewCannotResolveTypeException(type);

        private static Exception NewCannotResolveTypeException(Type typeToResolve)
        {
            var messageFormat = ExceptionMessages.MessageHandlerFactoryStub_CannotResolveMessageHandler;
            var message = string.Format(messageFormat,                
                typeToResolve.FriendlyName(),
                nameof(MicroProcessor),
                nameof(MicroProcessor.CreateMessageHandlerFactory),
                nameof(MessageHandlerFactory));

            return new NotSupportedException(message);
        }

        #endregion

        #region [====== Write ======]

        protected override MessageHandlerFactory RegisterPerResolve(Type from, Type to) =>
            this;

        protected override MessageHandlerFactory RegisterPerUnitOfWork(Type from, Type to) =>
            this;

        protected override MessageHandlerFactory RegisterPerProcessor(Type from, Type to) =>
            this;

        public override MessageHandlerFactory RegisterInstance(Type from, object to) =>
            this;

        #endregion        
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerFilterPipeline : MessageHandler
    {
        #region [====== HandleAsyncMethod ======]

        private sealed class HandleAsyncMethod : MessageHandlerOrQueryMethod<MessageStream>
        {
            private readonly MessageHandlerFilterPipeline _connector;
            private readonly IMicroProcessorFilter _filter;

            public HandleAsyncMethod(MessageHandlerFilterPipeline connector, IMicroProcessorFilter filter)
            {
                _connector = connector;
                _filter = filter;
            }

            public IMicroProcessorFilter Filter =>
                _filter;

            private MessageHandlerOrQueryMethod<MessageStream> Method =>
                _connector._nextHandler.Method;

            public override MethodInfo Info =>
                Method.Info;

            public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
                Method.TryGetAttributeOfType(out attribute);

            public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
                Method.GetAttributesOfType<TAttribute>();

            public override Task<InvokeAsyncResult<MessageStream>> InvokeAsync() =>
                _filter.InvokeMessageHandlerAsync(_connector._nextHandler);

            public override string ToString() =>
                Method.ToString();
        }

        #endregion

        private readonly MessageHandler _nextHandler;
        private readonly HandleAsyncMethod _method;

        public MessageHandlerFilterPipeline(MessageHandler nextHandler, IMicroProcessorFilter filter)             
        {
            _nextHandler = nextHandler;
            _method = new HandleAsyncMethod(this, filter);
        }

        public override string ToString() =>
            $"{_method.Filter.GetType().FriendlyName()} | {_nextHandler}";

        #region [====== IAttributeProvider<Type> ======]

        public override Type Type =>
            _nextHandler.Type;

        public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
            _nextHandler.TryGetAttributeOfType(out attribute);

        public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
            _nextHandler.GetAttributesOfType<TAttribute>();

        #endregion

        #region [====== IMessageHandlerOrQuery<MessageStream> ======]

        public override MessageHandlerContext Context =>
            _nextHandler.Context;

        public override MessageHandlerOrQueryMethod<MessageStream> Method =>
            _method;

        #endregion
    }
}

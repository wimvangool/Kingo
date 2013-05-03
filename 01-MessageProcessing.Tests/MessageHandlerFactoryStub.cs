using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageHandlerFactoryStub : MessageHandlerFactory
    {
        private readonly List<Type> _perResolveLifetimeTypes;
        private readonly List<Type> _perUnitOfWorkLifetimeTypes;
        private readonly List<Type> _singleLifetimeTypes;

        public MessageHandlerFactoryStub()
        {
            _perResolveLifetimeTypes = new List<Type>(4);
            _perUnitOfWorkLifetimeTypes = new List<Type>(4);
            _singleLifetimeTypes = new List<Type>(4);
        }

        public bool HasRegistered(Type type, InstanceLifetime lifetime)
        {
            switch (lifetime)
            {
                case InstanceLifetime.PerResolve:
                    return _perResolveLifetimeTypes.Contains(type);

                case InstanceLifetime.PerUnitOfWork:
                    return _perUnitOfWorkLifetimeTypes.Contains(type);

                case InstanceLifetime.Single:
                    return _singleLifetimeTypes.Contains(type);

                default:
                    return false;
            }            
        }

        protected internal override void RegisterWithPerResolveLifetime(Type type)
        {
            _perResolveLifetimeTypes.Add(type);
        }

        protected internal override void RegisterWithPerUnitOfWorkLifetime(Type type)
        {
            _perUnitOfWorkLifetimeTypes.Add(type);
        }

        protected internal override void RegisterSingle(Type type)
        {
            _singleLifetimeTypes.Add(type);
        }        

        protected internal override object Resolve(Type type)
        {
            throw new NotSupportedException("This method is not supported by the stub.");
        }
    }
}

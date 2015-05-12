using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerFactoryStub : MessageHandlerFactory
    {
        private readonly List<Type> _perResolveLifetimeTypes;
        private readonly List<Type> _perUnitOfWorkLifetimeTypes;        
        private readonly List<Type> _singleLifetimeTypes;

        public MessageHandlerFactoryStub()
        {
            _perResolveLifetimeTypes = new List<Type>();
            _perUnitOfWorkLifetimeTypes = new List<Type>();            
            _singleLifetimeTypes = new List<Type>();
        }

        public bool HasRegistered(Type type, InstanceLifetime lifetime)
        {
            switch (lifetime)
            {
                case InstanceLifetime.PerResolve:
                    return _perResolveLifetimeTypes.Contains(type);

                case InstanceLifetime.PerUnitOfWork:
                    return _perUnitOfWorkLifetimeTypes.Contains(type);                

                case InstanceLifetime.Singleton:
                    return _singleLifetimeTypes.Contains(type);

                default:
                    return false;
            }            
        }

        protected internal override void RegisterWithPerResolveLifetime(Type concreteType)
        {
            _perResolveLifetimeTypes.Add(concreteType);
        }

        protected internal override void RegisterWithPerResolveLifetime(Type concreteType, Type abstractType)
        {
            throw NewNotSupportedException();
        }

        protected internal override void RegisterWithPerUnitOfWorkLifetime(Type type)
        {
            _perUnitOfWorkLifetimeTypes.Add(type);
        }

        protected internal override void RegisterWithPerUnitOfWorkLifetime(Type concreteType, Type abstractType)
        {
            throw NewNotSupportedException();
        }        

        protected internal override void RegisterSingleton(Type concreteType)
        {
            _singleLifetimeTypes.Add(concreteType);
        }

        protected internal override void RegisterSingleton(Type concreteType, Type abstractType)
        {
            throw NewNotSupportedException();
        }

        protected internal override object CreateMessageHandler(Type type)
        {
            throw NewNotSupportedException();
        }

        private static NotSupportedException NewNotSupportedException()
        {
            return new NotSupportedException("This method is not supported by the stub.");
        }
    }
}

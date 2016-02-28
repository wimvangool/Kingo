using System;
using System.Collections.Generic;

namespace Kingo.Messaging
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

        public override void RegisterWithPerResolveLifetime(Type concreteType, Type abstractType = null)
        {
            _perResolveLifetimeTypes.Add(concreteType);
        }        

        public override void RegisterWithPerUnitOfWorkLifetime(Type concreteType, Type abstractType = null)
        {
            _perUnitOfWorkLifetimeTypes.Add(concreteType);            
        }                

        public override void RegisterSingleton(Type concreteType, Type abstractType = null)
        {
            _singleLifetimeTypes.Add(concreteType);
        }

        public override void RegisterSingleton(object concreteType, Type abstractType = null)
        {
            throw NewNotSupportedException();
        }

        protected internal override object Resolve(Type type)
        {
            throw NewNotSupportedException();
        }

        private static NotSupportedException NewNotSupportedException()
        {
            return new NotSupportedException("This method is not supported by the stub.");
        }
    }
}

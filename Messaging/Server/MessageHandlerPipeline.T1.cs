using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Globalization;
using System.Linq;

namespace System.ComponentModel.Messaging.Server
{    
    internal abstract class MessageHandlerPipeline<TMessage> : IMessageHandlerPipeline<TMessage> where TMessage : class
    {                
        public abstract void Handle(TMessage message);        

        #region [====== Class Attributes ======]
        
        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return TryGetClassAttributeOfType(false, out attribute);
        }
        
        public bool TryGetClassAttributeOfType<TAttribute>(bool includeInherited, out TAttribute attribute) where TAttribute : class
        {
            object attributeAsObject;

            if (TryGetClassAttributeOfType(typeof(TAttribute), includeInherited, out attributeAsObject))
            {
                attribute = (TAttribute) attributeAsObject;
                return true;
            }
            attribute = null;
            return false;
        }
        
        public bool TryGetClassAttributeOfType(Type type, out object attribute)
        {
            return TryGetClassAttributeOfType(type, false, out attribute);
        }
        
        public bool TryGetClassAttributeOfType(Type type, bool includeInherited, out object attribute)
        {            
            var attributes = GetClassAttributesOfType(type, includeInherited);

            try
            {
                return (attribute = attributes.SingleOrDefault()) != null;
            }
            catch (InvalidOperationException)
            {
                throw NewMultipleAttributesOfSpecifiedTypeException(type);
            }
        }
        
        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>(bool includeInherited = false)
        {
            return GetClassAttributesOfType(typeof(TAttribute), includeInherited).Cast<TAttribute>();
        }
        
        public abstract IEnumerable<object> GetClassAttributesOfType(Type type, bool includeInherited = false);        

        #endregion

        #region [====== Method Attributes ======]
        
        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return TryGetMethodAttributeOfType(false, out attribute);
        }
        
        public bool TryGetMethodAttributeOfType<TAttribute>(bool includeInherited, out TAttribute attribute) where TAttribute : class
        {            
            object attributeAsObject;

            if (TryGetMethodAttributeOfType(typeof(TAttribute), includeInherited, out attributeAsObject))
            {
                attribute = (TAttribute) attributeAsObject;
                return true;
            }
            attribute = null;
            return false;
        }
        
        public bool TryGetMethodAttributeOfType(Type type, out object attribute)
        {
            return TryGetMethodAttributeOfType(type, false, out attribute);
        }
        
        public bool TryGetMethodAttributeOfType(Type type, bool includeInherited, out object attribute)
        {            
            var attributes = GetMethodAttributesOfType(type, includeInherited);

            try
            {
                return (attribute = attributes.SingleOrDefault()) != null;
            }
            catch (InvalidOperationException)
            {
                throw NewMultipleAttributesOfSpecifiedTypeException(type);
            }
        }
        
        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>(bool includeInherited = false)
        {
            return GetMethodAttributesOfType(typeof(TAttribute), includeInherited).Cast<TAttribute>();
        }
        
        public abstract IEnumerable<object> GetMethodAttributesOfType(Type type, bool includeInherited = false);        

        #endregion

        private static Exception NewMultipleAttributesOfSpecifiedTypeException(Type type)
        {
            var messageFormat = ExceptionMessages.MessageHanderPipeline_MultipleAttributesFound;
            var message = string.Format(CultureInfo.InvariantCulture, messageFormat, type);
            return new InvalidOperationException(message);
        }
    }
}

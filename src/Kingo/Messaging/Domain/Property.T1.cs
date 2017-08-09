using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    internal sealed class Property<TValue> : Property
    {
        private readonly Type _type;
        private readonly string _name;        
        private readonly Func<object, TValue> _getMethod;
        private readonly Action<object, TValue> _setMethod;

        private Property(Type type, string name, Func<object, TValue> getMethod, Action<object, TValue> setMethod)            
        {
            _type = type;
            _name = name;            

            _getMethod = getMethod;
            _setMethod = setMethod;
        }

        public override string Name =>
            _name;

        public override string ToString() =>
            $"{_type.FriendlyName()}.{_name}";

        public TValue GetValue(object instance) =>
            _getMethod.Invoke(instance);

        public void SetValue(object instance, TValue value) =>
            _setMethod.Invoke(instance, value);

        public static Property<TValue> CreateProperty(Type type, Type attributeType)
        {
            var property = FindProperty(type, attributeType);
            if (property.GetIndexParameters().Length > 0)
            {
                throw NewIndexerPropertyFoundException(type, attributeType, property.Name);
            }
            if (property.PropertyType != typeof(TValue))
            {
                throw NewWrongPropertyTypeException(type, attributeType, property.Name);
            }
            var getMethod = property.GetGetMethod(true);
            if (getMethod == null)
            {
                throw NewMissingGetMethodException(type, attributeType, property.Name);
            }
            var setMethod = property.GetSetMethod(true);
            if (setMethod == null)
            {
                throw NewMissingSetMethodException(type, attributeType, property.Name);
            }
            return CreateProperty(type, property.Name, getMethod, setMethod);
        }        

        private static Property<TValue> CreateProperty(Type type, string name, MethodInfo getMethod, MethodInfo setMethod) =>
            new Property<TValue>(type, name, CreateGetMethod(type, getMethod), CreateSetMethod(type, setMethod));

        private static PropertyInfo FindProperty(Type type, Type attributeType)
        {
            var properties = FindCandidateProperties(type, attributeType).ToArray();
            if (properties.Length == 0)
            {
                throw NewPropertyNotFoundException(type, attributeType);
            }
            if (properties.Length > 1)
            {
                throw NewTooManyPropertiesFoundException(type, attributeType, string.Join(", ", properties.Select(property => property.Name)));
            }            
            return properties[0];
        }        

        private static IEnumerable<PropertyInfo> FindCandidateProperties(Type type, Type attributeType)
        {
            return
                from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where HasAttribute(property, attributeType)
                select property;
        }

        private static bool HasAttribute(MemberInfo property, Type attributeType) =>
            property.GetCustomAttributes(attributeType).Any();

        private static Func<object, TValue> CreateGetMethod(Type type, MethodInfo getMethod)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var instance = Expression.Convert(instanceParameter, type);
            var getterInvocation = Expression.Call(instance, getMethod);

            return Expression.Lambda<Func<object, TValue>>(getterInvocation, instanceParameter).Compile();
        }

        private static Action<object, TValue> CreateSetMethod(Type type, MethodInfo setMethod)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var instance = Expression.Convert(instanceParameter, type);
            var valueParameter = Expression.Parameter(typeof(TValue), "value");            
            var setterInvocation = Expression.Call(instance, setMethod, valueParameter);

            return Expression.Lambda<Action<object, TValue>>(setterInvocation, instanceParameter, valueParameter).Compile();
        }

        private static Exception NewPropertyNotFoundException(Type type, Type attributeType)
        {
            var messageFormat = ExceptionMessages.Property_PropertyNotFound;
            var message = string.Format(messageFormat, attributeType.FriendlyName(), type.FriendlyName());
            return NewInvalidOperationException(message);
        }

        private static Exception NewTooManyPropertiesFoundException(Type type, Type attributeType, string propertyList)
        {
            var messageFormat = ExceptionMessages.Property_TooManyPropertiesFound;
            var message = string.Format(messageFormat, attributeType.FriendlyName(), type.FriendlyName(), propertyList);
            return NewInvalidOperationException(message);
        }

        private static Exception NewIndexerPropertyFoundException(Type type, Type attributeType, string propertyName)
        {
            var messageFormat = ExceptionMessages.Property_PropertyIndexerFound;
            var message = string.Format(messageFormat, attributeType.FriendlyName(), type.FriendlyName(), propertyName);
            return NewInvalidOperationException(message);
        }

        private static Exception NewWrongPropertyTypeException(Type type, Type attributeType, string propertyName)
        {
            var messageFormat = ExceptionMessages.Property_WrongPropertyType;
            var message = string.Format(messageFormat, attributeType.FriendlyName(), type.FriendlyName(), propertyName, typeof(TValue).FriendlyName());
            return NewInvalidOperationException(message);
        }

        private static Exception NewMissingGetMethodException(Type type, Type attributeType, string propertyName)
        {
            var messageFormat = ExceptionMessages.Property_MissingGetMethod;
            var message = string.Format(messageFormat, attributeType.FriendlyName(), type.FriendlyName(), propertyName);
            return NewInvalidOperationException(message);
        }

        private static Exception NewMissingSetMethodException(Type type, Type attributeType, string propertyName)
        {
            var messageFormat = ExceptionMessages.Property_MissingSetMethod;
            var message = string.Format(messageFormat, attributeType.FriendlyName(), type.FriendlyName(), propertyName);
            return NewInvalidOperationException(message);
        }

        private static Exception NewInvalidOperationException(string message) =>
            new InvalidOperationException($"{message} {ExceptionMessages.Property_InstructionInfo}");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class ServiceContract : IReadOnlyCollection<Type>
    {
        private readonly HashSet<Type> _messageTypes;

        private ServiceContract(IEnumerable<Type> messageTypes)
        {
            _messageTypes = new HashSet<Type>(messageTypes);
        }

        public int Count =>
            _messageTypes.Count;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<Type> GetEnumerator() =>
            _messageTypes.GetEnumerator();

        public bool Contains(Type messageType) =>
            _messageTypes.Contains(messageType);

        public static ServiceContract FromTypeSet(IEnumerable<Type> messageTypes) =>
            new ServiceContract(messageTypes.Where(IsMessageType));

        private static bool IsMessageType(Type type) =>
            type.IsPublic && type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters;
    }
}

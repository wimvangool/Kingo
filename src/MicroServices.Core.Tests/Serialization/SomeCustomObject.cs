using System;
using System.Runtime.Serialization;

namespace Kingo.Serialization
{
    [Serializable]
    [DataContract]
    internal sealed class SomeCustomObject
    {
        public SomeCustomObject()
        {
            Id = Guid.NewGuid();
        }

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }
    }
}

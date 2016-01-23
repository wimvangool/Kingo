using System;
using System.Runtime.Serialization;

namespace Kingo.Messaging
{    
    [DataContract]
    [Serializable]
    internal sealed class NullMessage : Message, IEquatable<NullMessage>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as NullMessage);
        }

        public bool Equals(NullMessage other)
        {
            return other != null;
        }

        public override int GetHashCode()
        {
            return typeof(NullMessage).GetHashCode();
        }

        /// <inheritdoc />
        public override Message Copy()
        {
            return new NullMessage();
        }    

        internal static NullMessage Instance = new NullMessage();
    }
}

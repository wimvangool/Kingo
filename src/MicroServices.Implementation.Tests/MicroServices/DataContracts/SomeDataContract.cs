using System.Runtime.Serialization;

namespace Kingo.MicroServices.DataContracts
{
    [DataContract]
    internal sealed class SomeDataContract
    {
        [DataMember]
        public int Value
        {
            get;
            set;
        }

        public override bool Equals(object? obj) =>
            Equals(obj as SomeDataContract);

        public bool Equals(SomeDataContract other) =>
            !ReferenceEquals(other, null) && Value == other.Value;

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }
}

using System;

namespace Kingo.Messaging.Domain
{
    public sealed class OldValueChangedEvent : Event
    {
        private readonly Guid _id;
        private readonly int _version;
        private readonly int _value;

        public OldValueChangedEvent(Guid id, int version, int value)
        {
            _id = id;
            _version = version;
            _value = value;
        }

        protected override IEvent UpdateToLatestVersion() => new ValueChangedEvent
        {
            Id = _id,
            Version = _version,
            NewValue = _value
        };
    }
}

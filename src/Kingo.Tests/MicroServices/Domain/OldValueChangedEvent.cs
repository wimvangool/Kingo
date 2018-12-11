using System;

namespace Kingo.MicroServices.Domain
{
    public sealed class OldValueChangedEvent : Event<Guid, int>
    {        
        private readonly int _value;

        public OldValueChangedEvent(Guid id, int version, int value) :
            base(id, version)
        {            
            _value = value;
        }

        protected override IEvent UpdateToLatestVersion() =>
            new ValueChangedEvent(Id, Version) { NewValue = _value };       
    }
}

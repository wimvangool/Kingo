using System;

namespace Kingo.MicroServices.Domain
{
    internal sealed class AddValueCommand
    {
        public AddValueCommand(Guid numberId, int value)
        {
            NumberId = numberId;
            Value = value;
        }

        public Guid NumberId
        {
            get;
        }

        public int Value
        {
            get;
        }

        public static AddValueCommand Random() =>
            new AddValueCommand(Guid.NewGuid(), DateTimeOffset.UtcNow.Second);
    }
}

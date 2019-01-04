using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal sealed class CreateNumberCommand
    {
        public CreateNumberCommand(Guid numberId, int value)
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

        public static CreateNumberCommand Random() =>
            new CreateNumberCommand(Guid.NewGuid(), DateTimeOffset.UtcNow.Second);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

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
    }
}

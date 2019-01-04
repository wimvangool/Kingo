using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal sealed class DeleteNumberCommand
    {
        public DeleteNumberCommand(Guid numberId)
        {
            NumberId = numberId;            
        }

        public Guid NumberId
        {
            get;
        }

        public static DeleteNumberCommand Random() =>
            new DeleteNumberCommand(Guid.NewGuid());
    }
}

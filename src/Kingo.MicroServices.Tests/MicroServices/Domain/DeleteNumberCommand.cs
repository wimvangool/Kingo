using System;

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

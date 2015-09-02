using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    public sealed class EmptyMessage : Message<EmptyMessage>
    {
        public override EmptyMessage Copy()
        {
            return new EmptyMessage();
        }
    }
}

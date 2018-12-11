using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal interface IRemovedAttributeCollection
    {
        bool Contains(Type attributeType);
    }
}

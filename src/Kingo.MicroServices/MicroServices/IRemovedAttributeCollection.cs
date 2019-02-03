using System;

namespace Kingo.MicroServices
{
    internal interface IRemovedAttributeCollection
    {
        bool Contains(Type attributeType);
    }
}

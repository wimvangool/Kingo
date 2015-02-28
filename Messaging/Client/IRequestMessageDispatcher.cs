using System.Collections.Generic;

namespace System.ComponentModel.Client
{
    internal interface IRequestMessageDispatcher
    {
        IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>() where TAttribute : Attribute;
    }
}

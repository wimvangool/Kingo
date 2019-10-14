using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal interface IMessageIdFactory
    {
        string GenerateMessageIdFor(object content);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal interface IInstanceCollector
    {
        void Add(object instance);

        void AssertInstanceCountIs(int count);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class QueryTestOperation : MicroProcessorTestOperation
    {
        public abstract Type QueryType
        {
            get;
        }
    }
}

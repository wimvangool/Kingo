using System;

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

using System;

namespace Kingo.MicroServices
{
    internal abstract class QueryTestOperation : MicroProcessorTestOperation
    {
        public abstract Type QueryType
        {
            get;
        }
    }
}

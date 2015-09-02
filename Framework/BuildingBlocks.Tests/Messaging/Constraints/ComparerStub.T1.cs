using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal sealed class ComparerStub<T> : IComparer<T>
    {
        private readonly int _result;

        internal ComparerStub(int result)
        {
            _result = result;
        }

        public int Compare(T x, T y)
        {
            return _result;
        }
    }
}

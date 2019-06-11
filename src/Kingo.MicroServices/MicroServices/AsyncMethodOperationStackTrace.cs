using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    internal abstract class AsyncMethodOperationStackTrace : IAsyncMethodOperationStackTrace
    {
        #region [====== EmptyStackTrace ======]

        private sealed class EmptyStackTrace : AsyncMethodOperationStackTrace
        {
            public override int Count =>
                0;

            public override IAsyncMethodOperation this[int index] =>
                throw NewIndexOutOfRangeException(index, Count);            

            public override IEnumerator<IAsyncMethodOperation> GetEnumerator() =>
                Enumerable.Empty<IAsyncMethodOperation>().GetEnumerator();

            public override IAsyncMethodOperation CurrentOperation =>
                null;

            public override AsyncMethodOperationStackTrace Push(IAsyncMethodOperation operation) =>
                new SingleItemStackTrace(operation);
        }

        #endregion

        #region [====== SingleItemStackTrace ======]

        private sealed class SingleItemStackTrace : AsyncMethodOperationStackTrace
        {
            private readonly IAsyncMethodOperation _operation;

            public SingleItemStackTrace(IAsyncMethodOperation operation)
            {
                _operation = operation;
            }

            public override int Count =>
                1;

            public override IAsyncMethodOperation this[int index]
            {
                get
                {
                    if (index == 0)
                    {
                        return _operation;
                    }
                    throw NewIndexOutOfRangeException(index, Count);
                }
            }

            public override IEnumerator<IAsyncMethodOperation> GetEnumerator()
            {
                yield return _operation;
            }


            public override IAsyncMethodOperation CurrentOperation =>
                _operation;

            public override AsyncMethodOperationStackTrace Push(IAsyncMethodOperation operation) =>
                new RecursiveStackTrace(this, operation);
        }

        #endregion

        #region [====== RecursiveStackTrace ======]

        private sealed class RecursiveStackTrace : AsyncMethodOperationStackTrace
        {
            private readonly IAsyncMethodOperationStackTrace _stackTrace;
            private readonly IAsyncMethodOperation _operation;
            private readonly int _count;

            public RecursiveStackTrace(IAsyncMethodOperationStackTrace stackTrace, IAsyncMethodOperation operation)
            {
                _stackTrace = stackTrace;
                _operation = operation;
                _count = _stackTrace.Count + 1;
            }

            public override int Count =>
                _count;

            private int Index =>
                _count - 1;

            public override IAsyncMethodOperation this[int index]
            {
                get
                {
                    if (index == Index)
                    {
                        return _operation;
                    }
                    if (0 <= index && index < Index)
                    {
                        return _stackTrace[index];
                    }
                    throw NewIndexOutOfRangeException(index, Count);
                }
            }

            public override IEnumerator<IAsyncMethodOperation> GetEnumerator()
            {
                foreach (var operation in _stackTrace)
                {
                    yield return operation;
                }
                yield return _operation;
            }

            public override IAsyncMethodOperation CurrentOperation =>
                _operation;

            public override AsyncMethodOperationStackTrace Push(IAsyncMethodOperation operation) =>
                new RecursiveStackTrace(this, operation);
        }

        #endregion

        public static readonly AsyncMethodOperationStackTrace Empty = new EmptyStackTrace();

        public override string ToString() =>
            string.Join(" -> ", this.Select(operation => operation.ToString()));

        #region [====== IReadOnlyList<IAsyncMethodOperation> ======]

        public abstract int Count
        {
            get;
        }

        public abstract IAsyncMethodOperation this[int index]
        {
            get;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public abstract IEnumerator<IAsyncMethodOperation> GetEnumerator();

        #endregion

        #region [====== IAsyncMethodOperationStackTrace ======]

        public abstract IAsyncMethodOperation CurrentOperation
        {
            get;
        }

        public abstract AsyncMethodOperationStackTrace Push(IAsyncMethodOperation operation);

        private static Exception NewIndexOutOfRangeException(int index, int count)
        {
            var messageFormat = ExceptionMessages.ReadOnlyList_IndexOutOfRange;
            var message = string.Format(messageFormat, index, count);
            return new IndexOutOfRangeException(message);
        }

        #endregion        
    }
}

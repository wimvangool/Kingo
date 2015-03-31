using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace System.ComponentModel.Server
{
    internal sealed class UnitOfWorkController
    {
        #region [====== Nested Types =====]

        private sealed class AsynchronousFlushTask : IDisposable
        {
            private readonly Task[] _tasks;

            public AsynchronousFlushTask(IEnumerable<UnitOfWorkWrapper> units)
            {
                _tasks = CreateTasks(units);
            }

            public void Dispose()
            {
                foreach (var task in _tasks)
                {
                    task.Dispose();
                }
            }

            public void Start()
            {
                foreach (var task in _tasks)
                {
                    task.Start();
                }
            }

            public void Wait()
            {
                Task.WaitAll(_tasks);
            }

            private static Task[] CreateTasks(IEnumerable<UnitOfWorkWrapper> units)
            {
                var transaction = Transaction.Current;
                if (transaction == null)
                {
                    return CreateTasksWithoutTransaction(units);
                }
                return CreateTasksWithTransaction(units, transaction);
            }

            private static Task[] CreateTasksWithoutTransaction(IEnumerable<UnitOfWorkWrapper> units)
            {
                return units.Select(CreateTaskWithoutTransaction).ToArray();
            }

            private static Task CreateTaskWithoutTransaction(UnitOfWorkWrapper unit)
            {
                return new Task(unit.Flush);
            }

            private static Task[] CreateTasksWithTransaction(IEnumerable<UnitOfWorkWrapper> units, Transaction transaction)
            {
                return units.Select(unit => CreateTaskWithTransaction(unit, transaction)).ToArray();
            }

            private static Task CreateTaskWithTransaction(UnitOfWorkWrapper unit, Transaction transaction)
            {
                return new Task(() => FlushInTransaction(unit, transaction.DependentClone(DependentCloneOption.RollbackIfNotComplete)));
            }

            private static void FlushInTransaction(UnitOfWorkWrapper unit, DependentTransaction transaction)
            {
                using (var scope = new TransactionScope(transaction))
                {
                    unit.Flush();
                    scope.Complete();
                }
                transaction.Complete();
            }
        }

        #endregion

        private readonly List<UnitOfWorkWrapper> _enlistedUnits;        

        public UnitOfWorkController()
        {
            _enlistedUnits = new List<UnitOfWorkWrapper>(2);            
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal int EnlistedItemCount
        {
            get { return _enlistedUnits.Sum(unit => unit.ItemCount); }
        }

        public override string ToString()
        {
            return string.Format("{0} Item(s) Enlisted", EnlistedItemCount);
        }

        public void Enlist(IUnitOfWork unitOfWork)
        {
            Enlist(new UnitOfWorkItem(unitOfWork));
        }

        private void Enlist(UnitOfWorkItem newUnit)
        {
            // New units are first attempted to be merged with any existing unit based on group-id,
            // since units with the same group-id will always be flushed on same thread sequentially (but not
            // in any particular order). Only if this fails will units be added as a completely new item.
            for (int index = 0; index < _enlistedUnits.Count; index++)
            {
                UnitOfWorkWrapper unit = _enlistedUnits[index];
                UnitOfWorkGroup unitGroup;

                if (unit.WrapsSameUnitOfWorkAs(newUnit))
                {
                    return;
                }
                if (unit.TryMergeWith(newUnit, out unitGroup))
                {
                    _enlistedUnits[index] = unitGroup;
                    return;
                }
            }
            _enlistedUnits.Add(newUnit);
        }

        public void Flush()
        {
            LinkedList<UnitOfWorkWrapper> unitsOfWork;

            while (HasToFlush(out unitsOfWork))
            {
                Flush(unitsOfWork);
            }                        
        }

        private bool HasToFlush(out LinkedList<UnitOfWorkWrapper> unitsOfWork)
        {
            // First, all unit-items and groups that actually require a flush are collected.
            unitsOfWork = new LinkedList<UnitOfWorkWrapper>();

            foreach (var unit in _enlistedUnits)
            {
                unit.CollectUnitsThatRequireFlush(unitsOfWork);
            }
            _enlistedUnits.Clear();

            return unitsOfWork.Count > 0;
        }

        private static void Flush(LinkedList<UnitOfWorkWrapper> units)
        {
            // If only a single unit of work must be flushed, then a synchronous flush it will be.
            // This is because it makes no sense to flush a single
            // unit on a separate thread while this thread it waiting for it to finish.
            if (units.Count == 1)
            {
                FlushSynchronously(units);
            }
            else
            {
                FlushAsynchronouslyWherePossible(units);
            }
        }

        private static void FlushSynchronously(IEnumerable<UnitOfWorkWrapper> units)
        {
            foreach (var unitOfWork in units)
            {
                unitOfWork.Flush();
            }
        }

        private static void FlushAsynchronouslyWherePossible(LinkedList<UnitOfWorkWrapper> units)
        {            
            // First, we split the collection into units that allow themselves to be flushed asynchronously,
            // and the ones that force to be flushed on the current thread.
            LinkedList<UnitOfWorkWrapper> asyncUnits = RemoveResourcesThatCanBeFlushedAsynchronously(units);            

            // If all units are allowed to be flushed asynchronously, we will still flush one of them on
            // the current thread for the same reason mentioned above: it makes no sense to stall this thread
            // just waiting for another thread to finish it's work.
            if (units.Count == 0)
            {
                units.AddLast(asyncUnits.First.Value);
                asyncUnits.RemoveFirst();
            }
            // If no units allow themselves to be flushed on a different thread, then we'll fall back
            // to synchronous mode.
            if (asyncUnits.Count == 0)
            {
                FlushSynchronously(units);
                return;
            }
            // In the final case where some units can be flushed on different threads while at least a single
            // unit will be flushed on the current thread, we'll first kick-off the other threads to go ahead,
            // and then start flushing on the current thread. Should the current thread be done sooner than the other
            // ones, then we'll have to wait for them to finish up before returning.
            using (var task = new AsynchronousFlushTask(asyncUnits))
            {
                task.Start();

                FlushSynchronously(units);

                task.Wait();
            }
        }

        private static LinkedList<UnitOfWorkWrapper> RemoveResourcesThatCanBeFlushedAsynchronously(LinkedList<UnitOfWorkWrapper> units)
        {
            var asyncUnits = new LinkedList<UnitOfWorkWrapper>();
            var currentNode = units.First;

            do
            {
                if (currentNode.Value.CanBeFlushedAsynchronously)
                {
                    asyncUnits.AddLast(currentNode.Value);

                    var nextNode = currentNode.Next;
                    units.Remove(currentNode);
                    currentNode = nextNode;                      
                }
                else
                {
                    currentNode = currentNode.Next; 
                }
            }
            while (currentNode != null);

            return asyncUnits;
        }               
    }
}

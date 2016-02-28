using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class UnitOfWorkController
    {
        private readonly Queue<IUnitOfWork> _enlistedUnits;

        public UnitOfWorkController()
        {
            _enlistedUnits = new Queue<IUnitOfWork>();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal int EnlistedItemCount
        {
            get { return _enlistedUnits.Count; }
        }

        public override string ToString()
        {
            return string.Format("{0} Item(s) Enlisted", EnlistedItemCount);
        }

        public void Enlist(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            lock (_enlistedUnits)
            {
                _enlistedUnits.Enqueue(unitOfWork);
            }
        }

        internal async Task FlushAsync()
        {
            IEnumerable<IUnitOfWork> unitsOfWorkToFlush;

            while (HasToFlush(out unitsOfWorkToFlush))
            {
                await FlushAsync(unitsOfWorkToFlush);
            }
        }

        private bool HasToFlush(out IEnumerable<IUnitOfWork> unitsOfWorkToFlush)
        {
            if (_enlistedUnits.Count == 0)
            {
                unitsOfWorkToFlush = Enumerable.Empty<IUnitOfWork>();
                return false;
            }
            var unitsOfWorkToFlushList = new List<IUnitOfWork>();

            lock (_enlistedUnits)
            {
                while (_enlistedUnits.Count > 0)
                {
                    var unitOfWork = _enlistedUnits.Dequeue();
                    if (unitOfWork.RequiresFlush())
                    {
                        unitsOfWorkToFlushList.Add(unitOfWork);
                    }
                }
            }
            unitsOfWorkToFlush = unitsOfWorkToFlushList;

            return unitsOfWorkToFlushList.Count > 0;
        }

        private static Task FlushAsync(IEnumerable<IUnitOfWork> units)
        {
            return Task.WhenAll(units.Select(unit => unit.FlushAsync()));
        }
    }
}

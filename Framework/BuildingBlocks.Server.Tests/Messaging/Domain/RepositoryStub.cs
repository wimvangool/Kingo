using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging.Domain
{
    internal sealed class RepositoryStub : Repository<AggregateStub, Guid, int>
    {
        private readonly AggregateStub _aggregate;

        internal RepositoryStub()
        {
            _aggregate = null;
        }

        internal RepositoryStub(AggregateStub aggregate)
        {
            _aggregate = aggregate;
        }

        internal bool WasEnlisted
        {
            get;
            private set;
        }

        protected override void Enlist()
        {
            WasEnlisted = true;
        }

        public override Task FlushAsync()
        {
            WasEnlisted = false;

            return base.FlushAsync();
        }

        #region [====== SelectByKey ======]

        private readonly Dictionary<Guid, int> _selectedKeys = new Dictionary<Guid, int>();

        internal int SelectCountOf(Guid key)
        {
            return CountOf(key, _selectedKeys);
        }

        protected override Task<AggregateStub> SelectByKeyAsync(Guid key)
        {
            Add(key, _selectedKeys);

            return AsyncMethod.RunSynchronously(() =>
            {                              
                if (_aggregate != null && _aggregate.Id.Equals(key))
                {
                    return _aggregate;
                }
                return null;
            });
        }        

        #endregion

        #region [====== UpdateAsync ======]

        private readonly Dictionary<Guid, int> _updatedKeys = new Dictionary<Guid, int>();

        internal int UpdateCountOf(Guid key)
        {
            return CountOf(key, _updatedKeys);
        }

        protected override Task UpdateAsync(AggregateStub aggregate, int originalVersion)
        {
            Add(aggregate.Id, _updatedKeys);

            return AsyncMethod.Void;
        }

        #endregion

        #region [====== InsertAsync ======]

        private readonly Dictionary<Guid, int> _insertedKeys = new Dictionary<Guid, int>();

        internal int InsertCountOf(Guid key)
        {
            return CountOf(key, _insertedKeys);
        }

        protected override Task InsertAsync(AggregateStub aggregate)
        {
            Add(aggregate.Id, _insertedKeys);

            return AsyncMethod.Void;
        }

        #endregion

        #region [====== DeleteAsync ======]

        private readonly Dictionary<Guid, int> _deletedKeys = new Dictionary<Guid, int>();

        internal int DeleteCountOf(Guid key)
        {
            return CountOf(key, _deletedKeys);
        }

        protected override Task DeleteAsync(Guid key)
        {
            Add(key, _deletedKeys);

            return AsyncMethod.Void;
        }

        #endregion

        private static int CountOf(Guid key, IDictionary<Guid, int> keys)
        {
            int count;

            if (keys.TryGetValue(key, out count))
            {
                return count;
            }
            return 0;
        }

        private static void Add(Guid key, IDictionary<Guid, int> keys)
        {
            int count;

            if (keys.TryGetValue(key, out count))
            {
                keys[key] = count + 1;
            }
            else
            {
                keys.Add(key, 1);
            }
        }
    }
}

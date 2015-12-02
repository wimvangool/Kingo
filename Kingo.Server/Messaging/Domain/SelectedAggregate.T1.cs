using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    internal sealed class SelectedAggregate<TKey, TVersion, TAggregate> : Aggregate<TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>, IReadableEventStream<TKey, TVersion>
    {
        

        internal override bool IsUpdated
        {
            get { throw new NotImplementedException(); }
        }

        internal override TAggregate Value
        {
            get { return _aggregate; }
        }

        internal override Task CommitAsync()
        {
            throw new NotImplementedException();
        }
    }
}

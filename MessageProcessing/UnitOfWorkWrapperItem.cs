using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace YellowFlare.MessageProcessing
{
    internal sealed class UnitOfWorkWrapperItem : UnitOfWorkWrapper
    {
        private readonly IUnitOfWork _flushable;
        private readonly Lazy<FlushHintAttribute> _flushHintAttribute;

        public UnitOfWorkWrapperItem(IUnitOfWork flushable)
        {
            if (flushable == null)
            {
                throw new ArgumentNullException("flushable");
            }
            _flushable = flushable;
            _flushHintAttribute = new Lazy<FlushHintAttribute>(() => GetOrAddFlushHintAttribute(flushable.GetType()));
        }        

        public override string Group
        {
            get { return _flushHintAttribute.Value.Group; }
        }

        public override bool ForceSynchronousFlush
        {
            get { return _flushHintAttribute.Value.ForceSynchronousFlush; }
        }

        public override bool WrapsSameUnitOfWorkAs(UnitOfWorkWrapperItem wrapper)
        {
            Debug.Assert(wrapper != null);

            return ReferenceEquals(_flushable, wrapper._flushable);
        }

        protected override UnitOfWorkWrapperGroup MergeWith(UnitOfWorkWrapperItem wrapper)
        {
            return new UnitOfWorkWrapperGroup(this, wrapper);
        }

        public override void CollectWrappersThatRequireFlush(ICollection<UnitOfWorkWrapper> wrappers)
        {
            Debug.Assert(wrappers != null);

            if (_flushable.RequiresFlush())
            {
                wrappers.Add(this);
            }
        }

        public bool RequiresFlush()
        {
            return _flushable.RequiresFlush();
        }

        public override void Flush()
        {
            _flushable.Flush();
        }        

        private static readonly ConcurrentDictionary<Type, FlushHintAttribute> _FlushHintAttributes =
            new ConcurrentDictionary<Type, FlushHintAttribute>();

        private static FlushHintAttribute GetOrAddFlushHintAttribute(Type flushable)
        {
            return _FlushHintAttributes.GetOrAdd(flushable, GetFlushHintAttribute);
        }

        private static FlushHintAttribute GetFlushHintAttribute(Type flushable)
        {
            FlushHintAttribute attribute = flushable
                .GetCustomAttributes(typeof(FlushHintAttribute), true)
                .Cast<FlushHintAttribute>()
                .SingleOrDefault();

            return attribute ?? new FlushHintAttribute();
        }        
    }
}

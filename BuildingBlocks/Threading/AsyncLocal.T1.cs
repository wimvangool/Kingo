using System;
using System.Runtime.Remoting.Messaging;

namespace Kingo.BuildingBlocks.Threading
{
    /// <summary>
    /// Represents a memory slot inside the <see cref="LogicalCallContext" />.
    /// </summary>
    /// <typeparam name="T">Type of the value to store.</typeparam>
    public sealed class AsyncLocal<T>
    {
        private sealed class Wrapper<TValue> : MarshalByRefObject
        {
            internal readonly TValue Value;

            internal Wrapper(TValue value)
            {
                Value = value;
            }
        }

        private readonly string _key;
        private readonly T _defaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLocal{T}" /> class.
        /// </summary>
        public AsyncLocal()
            : this(default(T)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLocal{T}" /> class.
        /// </summary>
        /// <param name="defaultValue">The default value of this slot.</param>
        public AsyncLocal(T defaultValue)
        {
            _key = Guid.NewGuid().ToString("N");
            _defaultValue = defaultValue;            
        }

        /// <summary>
        /// Gets or sets the value of the memory slot.
        /// </summary>
        public T Value
        {
            get
            {
                var wrapper = CallContext.LogicalGetData(_key) as Wrapper<T>;
                if (wrapper == null)
                {
                    return _defaultValue;
                }
                return wrapper.Value;
            }
            set { CallContext.LogicalSetData(_key, new Wrapper<T>(value)); }
        }
    }
}

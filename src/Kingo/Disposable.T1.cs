using System;
using System.Threading;

namespace Kingo
{
    /// <summary>
    /// Represents a disposable instance that is only created when needed.
    /// </summary>
    /// <typeparam name="TValue">Type of the disposable instance.</typeparam>
    public sealed class Disposable<TValue> : Disposable where TValue : class, IDisposable
    {
        private readonly Lazy<TValue> _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable{T}" /> class.
        /// </summary>
        /// <param name="valueFactory">Delegate used to create the instance when needed.</param>
        /// <param name="threadSafetyMode">
        /// Indicates the level of thread-safety required when creating the instance.
        /// </param>
        public Disposable(Func<TValue> valueFactory, LazyThreadSafetyMode threadSafetyMode = LazyThreadSafetyMode.None)
        {
            _value = new Lazy<TValue>(valueFactory, threadSafetyMode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable{T}" /> class.
        /// </summary>
        /// <param name="valueFactory">Delegate used to create the instance when needed.</param>
        /// <param name="isThreadSafe">
        /// Indicates whether or not the instance must be created in a thread-safe fashion, guaranteing only one instance
        /// to be created.
        /// </param>
        public Disposable(Func<TValue> valueFactory, bool isThreadSafe)
        {
            _value = new Lazy<TValue>(valueFactory, isThreadSafe);
        }

        /// <summary>
        /// Indicates whether or not the instance has been created.
        /// </summary>
        public bool IsValueCreated
        {
            get { return _value.IsValueCreated; }
        }

        /// <summary>
        /// Returns the instance. If it was not yet created, it is created on the spot.
        /// Returns <c>null</c> if this instance has already been disposed.
        /// </summary>
        public TValue Value
        {
            get
            {
                if (IsDisposed && !IsValueCreated)
                {
                    return null;
                }
                return _value.Value;
            }
        }

        /// <summary>
        /// Dispose the instance, if it has ben created.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            if (IsValueCreated)
            {
                Value.Dispose();
            }
            base.DisposeManagedResources();
        }
    }
}

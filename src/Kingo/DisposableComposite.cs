using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo
{
    /// <summary>
    /// Represents a composite of disposables that need to be disposed together.
    /// </summary>
    public class DisposableComposite : Disposable
    {
        private readonly IDisposable[] _disposables;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableComposite" /> class.
        /// </summary>
        /// <param name="disposables">A collection of disposables.</param>
        public DisposableComposite(params IDisposable[] disposables)
        {
            _disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableComposite" /> class.
        /// </summary>
        /// <param name="disposables">A collection of disposables.</param>
        public DisposableComposite(IEnumerable<IDisposable> disposables)
        {
            _disposables = disposables.ToArray();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} ({_disposables.Length} component(s))";
    }
}

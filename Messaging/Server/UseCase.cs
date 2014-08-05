using System.Threading;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents the stack of messages that is being handled by the processor.
    /// </summary>
    public sealed class UseCase : IProgressReporter
    {
        /// <summary>
        /// The message that is associated to this <see cref="UseCase" />.
        /// </summary>
        public readonly object Message;              

        /// <summary>
        /// The parent UseCase.
        /// </summary>
        public readonly UseCase ParentUseCase;

        private readonly CancellationToken? _token;
        private readonly IProgressReporter _reporter;
        
        internal UseCase(object message, CancellationToken? token, IProgressReporter reporter)
        {
            Message = message;

            _token = token;
            _reporter = reporter;
        }

        private UseCase(object message, CancellationToken? token, IProgressReporter reporter, UseCase parentUseCase)
        {            
            Message = message;
            ParentUseCase = parentUseCase;

            _token = token;
            _reporter = reporter;
        }

        internal UseCase CreateChildUseCase(object instance, CancellationToken? token, IProgressReporter reporter)
        {
            return new UseCase(instance, token, reporter, this);
        }

        /// <summary>
        /// Indicates whether or not <see cref="Message" /> is of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">Type to check.</typeparam>
        /// <returns><c>true</c> if this message is of the specified type; otherwise <c>false</c>.</returns>
        public bool IsMessageA<TMessage>()
        {
            return IsMessageA(typeof(TMessage));
        }

        /// <summary>
        /// Indicates whether or not <see cref="Message" /> is of the specified type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><c>true</c> if this message is of the specified type; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool IsMessageA(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.IsInstanceOfType(Message);
        }

        #region [====== Cancellation ======]

        /// <summary>
        /// Traverses up the stack looking for a token that indicates that a cancellation is requested and
        /// if found, throws an <see cref="OperationCanceledException" />.
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            UseCase stack = this;

            do
            {
                stack._token.ThrowIfCancellationRequested();
            } while ((stack = stack.ParentUseCase) != null);
        }

        #endregion

        #region [====== ProgressReporter ======]

        /// <inheritdoc />
        public void Report(int total, int progress)
        {
            if (_reporter != null)
            {
                _reporter.Report(total, progress);
            }
        }

        /// <inheritdoc />
        public void Report(double total, double progress)
        {
            if (_reporter != null)
            {
                _reporter.Report(total, progress);
            }
        }

        /// <inheritdoc />
        public void Report(Progress progress)
        {
            if (_reporter != null)
            {
                _reporter.Report(progress);
            }
        }

        #endregion
    }
}

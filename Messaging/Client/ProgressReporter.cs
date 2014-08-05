﻿using System.ComponentModel.Messaging.Server;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IProgressReporter" /> interface, where
    /// progress is reported back through an associated <see cref="SynchronizationContext" />.
    /// </summary>
    public class ProgressReporter : IProgressReporter
    {
        private readonly SynchronizationContext _synchronizationContext;
        private Progress _progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressReporter" /> class.
        /// </summary>
        public ProgressReporter()
        {
            _synchronizationContext = SynchronizationContext.Current;
            _progress = Progress.MinValue;
        }

        /// <summary>
        /// Returns the associated <see cref="SynchronizationContext" /> that is used to report back the progress.
        /// </summary>
        protected SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext; }
        }

        /// <summary>
        /// Occurs when <see cref="Progress" /> changed.
        /// </summary>
        public event EventHandler<Server.ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Raises the <see cref="ProgressChanged" /> event.
        /// </summary>
        protected virtual void OnProgressChanged(Server.ProgressChangedEventArgs e)
        {
            ProgressChanged.Raise(this, e);
        }

        /// <summary>
        /// Gets or sets the progress that has been made.
        /// </summary>
        public Progress Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;

                    OnProgressChanged(new Server.ProgressChangedEventArgs(value));
                }
            }
        }

        #region [====== IProgressReporter ======]       

        void IProgressReporter.Report(int total, int progress)
        {
            Report(Progress.Calculate(total, progress));
        }

        void IProgressReporter.Report(double total, double progress)
        {
            Report(Progress.Calculate(total, progress));
        }        

        void IProgressReporter.Report(Progress progress)
        {
            Report(progress);
        }

        private void Report(Progress progress)
        {
            using (var scope = new SynchronizationContextScope(_synchronizationContext))
            {
                scope.Post(() => Progress = progress);
            }
        }

        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="INotifyIsBusy" /> that is composed of other indicators.
    /// </summary>
    public class IsBusyIndicator : PropertyChangedBase, INotifyIsBusy, IEnumerable<INotifyIsBusy>
    {
        private readonly List<INotifyIsBusy> _indicators;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsBusyIndicator" /> class.
        /// </summary>
        public IsBusyIndicator()
        {
            _indicators = new List<INotifyIsBusy>();
        }       

        /// <summary>
        /// Initializes a new instance of the <see cref="IsBusyIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public IsBusyIndicator(IEnumerable<INotifyIsBusy> indicators)
        {
            if (indicators == null)
            {
                throw new ArgumentNullException("indicators");
            }
            _indicators = new List<INotifyIsBusy>();

            foreach (var indicator in indicators)
            {
                Add(indicator, false);
            }
        }

        #region [====== IsBusyIndicator ======]

        /// <inheritdoc />
        public event EventHandler IsBusyChanged;

        private void OnIsBusyChanged()
        {
            OnIsBusyChanged(this, EventArgs.Empty);
        }

        private void OnIsBusyChanged(object sender, EventArgs e)
        {
            IsBusyChanged.Raise(sender, e);

            NotifyOfPropertyChange(() => IsBusy);
        }

        /// <inheritdoc />
        public bool IsBusy
        {
            get { return _indicators.Any(indicator => indicator.IsBusy); }
        }

        #endregion

        #region [====== Adding and Removing of Indicators ======]        

        /// <summary>
        /// If not <c>null</c>, adds the specified <paramref name="indicator"/> to this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to add.</param>
        /// <remarks>
        /// <paramref name="indicator"/> is only added if it not <c>null</c> and not already present.
        /// </remarks>
        public void Add(INotifyIsBusy indicator)
        {
            Add(indicator, true);
        }

        private void Add(INotifyIsBusy indicator, bool raiseIsBusyChanged)
        {
            if (indicator == null || _indicators.Contains(indicator))
            {
                return;
            }
            _indicators.Add(indicator);

            indicator.IsBusyChanged += OnIsBusyChanged;

            if (raiseIsBusyChanged)
            {
                OnIsBusyChanged();
            }
        }

        /// <summary>
        /// Removes the specified <paramref name="indicator"/> from this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to remove.</param>
        public void Remove(INotifyIsBusy indicator)
        {
            Remove(indicator, true);
        }

        private void Remove(INotifyIsBusy indicator, bool raiseIsBusyChanged)
        {
            if (_indicators.Contains(indicator))
            {
                _indicators.Remove(indicator);

                indicator.IsBusyChanged -= OnIsBusyChanged;

                if (raiseIsBusyChanged)
                {
                    OnIsBusyChanged();
                }
            }
        }

        /// <summary>
        /// Removes all indicators from this indicator.
        /// </summary>
        public void Clear()
        {
            if (_indicators.Count == 0)
            {
                return;
            }
            var indicators = new List<INotifyIsBusy>(_indicators);
            
            foreach (var indicator in indicators)
            {
                Remove(indicator, false);
            }
            OnIsBusyChanged();
        }

        #endregion

        #region [====== Enumerable ======]

        public IEnumerator<INotifyIsBusy> GetEnumerator()
        {
            return _indicators.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _indicators.GetEnumerator();
        }

        #endregion
    }
}

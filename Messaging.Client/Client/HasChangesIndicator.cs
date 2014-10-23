using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="INotifyHasChanges" /> that is composed of other indicators.
    /// </summary>
    public class HasChangesIndicator : PropertyChangedBase, INotifyHasChanges, IEnumerable<INotifyHasChanges>
    {
        private readonly List<INotifyHasChanges> _indicators;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasChangesIndicator" /> class.
        /// </summary>
        public HasChangesIndicator()
        {
            _indicators = new List<INotifyHasChanges>();
        }                

        /// <summary>
        /// Initializes a new instance of the <see cref="HasChangesIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public HasChangesIndicator(IEnumerable<INotifyHasChanges> indicators)            
        {
            if (indicators == null)
            {
                throw new ArgumentNullException("indicators");
            }
            _indicators = new List<INotifyHasChanges>();

            foreach (var indicator in indicators)
            {
                Add(indicator, false);
            }
        }

        #region [====== HasChangesIndicator ======]

        /// <inheritdoc />
        public event EventHandler HasChangesChanged;

        private void OnHasChangesChanged()
        {
            OnHasChangesChanged(this,EventArgs.Empty);
        }

        private void OnHasChangesChanged(object sender, EventArgs e)
        {
            HasChangesChanged.Raise(sender, e);

            NotifyOfPropertyChange(() => HasChanges);
        }

        /// <inheritdoc />
        public bool HasChanges
        {
            get { return _indicators.Any(indicator => indicator.HasChanges); }
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
        public void Add(INotifyHasChanges indicator)
        {
            Add(indicator, true);
        }

        private void Add(INotifyHasChanges indicator, bool raiseHasChangesChanged)
        {
            if (indicator == null || _indicators.Contains(indicator))
            {
                return;
            }
            _indicators.Add(indicator);

            indicator.HasChangesChanged += OnHasChangesChanged;

            if (raiseHasChangesChanged)
            {
                OnHasChangesChanged();
            }
        }

        /// <summary>
        /// Removes the specified <paramref name="indicator"/> from this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to remove.</param>
        public void Remove(INotifyHasChanges indicator)
        {
            Remove(indicator, true);
        }

        private void Remove(INotifyHasChanges indicator, bool raiseHasChangesChanged)
        {
            if (_indicators.Remove(indicator))
            {
                indicator.HasChangesChanged -= OnHasChangesChanged;

                if (raiseHasChangesChanged)
                {
                    OnHasChangesChanged();
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
            var indicators = new List<INotifyHasChanges>(_indicators);
                       
            foreach (var indicator in indicators)
            {
                Remove(indicator, false);
            }
            OnHasChangesChanged();
        }

        #endregion

        #region [====== Enumerable ======]

        public IEnumerator<INotifyHasChanges> GetEnumerator()
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

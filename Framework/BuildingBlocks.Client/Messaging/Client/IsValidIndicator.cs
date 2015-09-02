using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.BuildingBlocks.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="INotifyIsValid" /> that is composed of other indicators.
    /// </summary>
    public class IsValidIndicator : PropertyChangedBase, INotifyIsValid, IEnumerable<INotifyIsValid>
    {
        private readonly List<INotifyIsValid> _indicators;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsValidIndicator" /> class.
        /// </summary>
        public IsValidIndicator()
        {
            _indicators = new List<INotifyIsValid>();
        }        

        /// <summary>
        /// Initializes a new instance of the <see cref="IsValidIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public IsValidIndicator(IEnumerable<INotifyIsValid> indicators)         
        {
            if (indicators == null)
            {
                throw new ArgumentNullException("indicators");
            }
            _indicators = new List<INotifyIsValid>();

            foreach (var indicator in indicators)
            {
                Add(indicator, false);
            }
        }

        #region [====== IsValidIndicator ======]

        /// <inheritdoc />
        public event EventHandler IsValidChanged;

        private void OnIsValidChanged()
        {
            OnIsValidChanged(this, EventArgs.Empty);
        }

        private void OnIsValidChanged(object sender, EventArgs e)
        {
            IsValidChanged.Raise(sender, e);

            NotifyOfPropertyChange(() => IsValid);
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get { return _indicators.All(indicator => indicator.IsValid); }
        }

        #endregion

        #region [====== Adding and Removing of Indicators ======]

        /// <summary>
        /// Adds all specified indicators to this indicator.
        /// </summary>
        /// <param name="indicators">The list of indicators to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public void AddRange(params INotifyIsValid[] indicators)
        {
            AddRange(indicators as IEnumerable<INotifyIsValid>);
        }

        /// <summary>
        /// Adds all specified indicators to this indicator.
        /// </summary>
        /// <param name="indicators">The list of indicators to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public void AddRange(IEnumerable<INotifyIsValid> indicators)
        {
            if (indicators == null)
            {
                throw new ArgumentNullException("indicators");
            }
            foreach (var indicator in indicators)
            {
                Add(indicator, false);
            }
            OnIsValidChanged();
        }

        /// <summary>
        /// If not <c>null</c>, adds the specified <paramref name="indicator"/> to this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to add.</param>
        /// <remarks>
        /// <paramref name="indicator"/> is only added if it not <c>null</c> and not already present.
        /// </remarks>
        public void Add(INotifyIsValid indicator)
        {
            Add(indicator, true);
        }

        private void Add(INotifyIsValid indicator, bool raiseIsValidChanged)
        {
            if (indicator == null || _indicators.Contains(indicator))
            {
                return;
            }
            _indicators.Add(indicator);

            indicator.IsValidChanged += OnIsValidChanged;
    
            if (raiseIsValidChanged)
            {
                OnIsValidChanged();
            }
        }

        /// <summary>
        /// Removes the specified <paramref name="indicator"/> from this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to remove.</param>
        public void Remove(INotifyIsValid indicator)
        {
            Remove(indicator, true);
        }

        private void Remove(INotifyIsValid indicator, bool raiseIsValidChanged)
        {
            if (_indicators.Contains(indicator))
            {
                _indicators.Remove(indicator);

                indicator.IsValidChanged -= OnIsValidChanged;

                if (raiseIsValidChanged)
                {
                    OnIsValidChanged();
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
            var indicators = new List<INotifyIsValid>(_indicators);
            
            foreach (var indicator in indicators)
            {
                Remove(indicator, false);
            }
            OnIsValidChanged();
        }

        #endregion

        #region [====== Enumerable ======]

        /// <inheritdoc />
        public IEnumerator<INotifyIsValid> GetEnumerator()
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

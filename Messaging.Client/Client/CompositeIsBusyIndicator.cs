using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="IIsBusyIndicator" /> that is composed of other indicators.
    /// </summary>
    public class CompositeIsBusyIndicator : PropertyChangedNotifier, IIsBusyIndicator
    {
        private readonly List<IIsBusyIndicator> _indicators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsBusyIndicator" /> class.
        /// </summary>
        public CompositeIsBusyIndicator()
        {
            _indicators = new List<IIsBusyIndicator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsBusyIndicator" /> class and adds the two
        /// specified indicators to this instance.
        /// </summary>        
        public CompositeIsBusyIndicator(IIsBusyIndicator a, IIsBusyIndicator b) : this()
        {
            Add(a);
            Add(b);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsBusyIndicator" /> class and adds the three
        /// specified indicators to this instance.
        /// </summary>        
        public CompositeIsBusyIndicator(IIsBusyIndicator a, IIsBusyIndicator b, IIsBusyIndicator c) : this()
        {
            Add(a);
            Add(b);
            Add(c);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsBusyIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public CompositeIsBusyIndicator(params IIsBusyIndicator[] indicators)
            : this(indicators as IEnumerable<IIsBusyIndicator>) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsBusyIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public CompositeIsBusyIndicator(IEnumerable<IIsBusyIndicator> indicators) : this()
        {
            AddRange(indicators);
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

            OnPropertyChanged(() => IsBusy);
        }

        /// <inheritdoc />
        public bool IsBusy
        {
            get { return _indicators.Any(indicator => indicator.IsBusy); }
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
        public void AddRange(params IIsBusyIndicator[] indicators)
        {
            AddRange(indicators as IEnumerable<IIsBusyIndicator>);
        }

        /// <summary>
        /// Adds all specified indicators to this indicator.
        /// </summary>
        /// <param name="indicators">The list of indicators to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public void AddRange(IEnumerable<IIsBusyIndicator> indicators)
        {
            if (indicators == null)
            {
                throw new ArgumentNullException("indicators");
            }
            foreach (var indicator in indicators)
            {
                Add(indicator, false);              
            }
            OnIsBusyChanged();
        }

        /// <summary>
        /// If not <c>null</c>, adds the specified <paramref name="indicator"/> to this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to add.</param>
        /// <remarks>
        /// <paramref name="indicator"/> is only added if it not <c>null</c> and not already present.
        /// </remarks>
        public void Add(IIsBusyIndicator indicator)
        {
            Add(indicator, true);
        }

        private void Add(IIsBusyIndicator indicator, bool raiseIsBusyChanged)
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
        public void Remove(IIsBusyIndicator indicator)
        {
            Remove(indicator, true);
        }

        private void Remove(IIsBusyIndicator indicator, bool raiseIsBusyChanged)
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
            var indicators = new List<IIsBusyIndicator>(_indicators);
            
            foreach (var indicator in indicators)
            {
                Remove(indicator, false);
            }
            OnIsBusyChanged();
        }

        #endregion
    }
}

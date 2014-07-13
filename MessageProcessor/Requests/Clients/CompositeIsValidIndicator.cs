using System;
using System.Collections.Generic;
using System.Linq;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    /// <summary>
    /// Represents a <see cref="IIsValidIndicator" /> that is composed of other indicators.
    /// </summary>
    public class CompositeIsValidIndicator : PropertyChangedNotifier, IIsValidIndicator
    {
        private readonly List<IIsValidIndicator> _indicators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsValidIndicator" /> class.
        /// </summary>
        public CompositeIsValidIndicator()
        {
            _indicators = new List<IIsValidIndicator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsValidIndicator" /> class and adds the two
        /// specified indicators to this instance.
        /// </summary>        
        public CompositeIsValidIndicator(IIsValidIndicator a, IIsValidIndicator b)
            : this()
        {
            Add(a);
            Add(b);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsValidIndicator" /> class and adds the three
        /// specified indicators to this instance.
        /// </summary>        
        public CompositeIsValidIndicator(IIsValidIndicator a, IIsValidIndicator b, IIsValidIndicator c)
            : this()
        {
            Add(a);
            Add(b);
            Add(c);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsValidIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public CompositeIsValidIndicator(params IIsValidIndicator[] indicators)
            : this(indicators as IEnumerable<IIsValidIndicator>) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeIsValidIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public CompositeIsValidIndicator(IEnumerable<IIsValidIndicator> indicators)
            : this()
        {
            Add(indicators);
        }

        #region [====== IsValidIndicator ======]

        /// <inheritdoc />
        public event EventHandler IsValidChanged;

        private void HandleIsValidChanged(object sender, EventArgs e)
        {
            IsValidChanged.Raise(sender, e);

            OnPropertyChanged(() => IsValid);
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get { return _indicators.Any(indicator => indicator.IsValid); }
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
        public void Add(params IIsValidIndicator[] indicators)
        {
            Add(indicators as IEnumerable<IIsValidIndicator>);
        }

        /// <summary>
        /// Adds all specified indicators to this indicator.
        /// </summary>
        /// <param name="indicators">The list of indicators to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public void Add(IEnumerable<IIsValidIndicator> indicators)
        {
            if (indicators == null)
            {
                throw new ArgumentNullException("indicators");
            }
            foreach (var indicator in indicators)
            {
                Add(indicator);
            }
        }

        /// <summary>
        /// If not <c>null</c>, adds the specified <paramref name="indicator"/> to this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to add.</param>
        /// <remarks>
        /// <paramref name="indicator"/> is only added if it not <c>null</c> and not already present.
        /// </remarks>
        public void Add(IIsValidIndicator indicator)
        {
            if (indicator == null || _indicators.Contains(indicator))
            {
                return;
            }
            _indicators.Add(indicator);

            indicator.IsValidChanged += HandleIsValidChanged;
        }

        /// <summary>
        /// Removes the specified <paramref name="indicator"/> from this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to remove.</param>
        public void Remove(IIsValidIndicator indicator)
        {
            if (_indicators.Contains(indicator))
            {
                _indicators.Remove(indicator);

                indicator.IsValidChanged -= HandleIsValidChanged;
            }
        }

        /// <summary>
        /// Removes all indicators from this indicator.
        /// </summary>
        public void Clear()
        {
            var indicators = new List<IIsValidIndicator>(_indicators);

            foreach (var indicator in indicators)
            {
                Remove(indicator);
            }
        }

        #endregion
    }
}

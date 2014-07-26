﻿using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="IHasChangesIndicator" /> that is composed of other indicators.
    /// </summary>
    public class CompositeHasChangesIndicator : PropertyChangedNotifier, IHasChangesIndicator
    {
        private readonly List<IHasChangesIndicator> _indicators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeHasChangesIndicator" /> class.
        /// </summary>
        public CompositeHasChangesIndicator()
        {
            _indicators = new List<IHasChangesIndicator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeHasChangesIndicator" /> class and adds the two
        /// specified indicators to this instance.
        /// </summary>        
        public CompositeHasChangesIndicator(IHasChangesIndicator a, IHasChangesIndicator b)
            : this()
        {
            Add(a);
            Add(b);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeHasChangesIndicator" /> class and adds the three
        /// specified indicators to this instance.
        /// </summary>        
        public CompositeHasChangesIndicator(IHasChangesIndicator a, IHasChangesIndicator b, IHasChangesIndicator c)
            : this()
        {
            Add(a);
            Add(b);
            Add(c);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeHasChangesIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public CompositeHasChangesIndicator(params IHasChangesIndicator[] indicators)
            : this(indicators as IEnumerable<IHasChangesIndicator>) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeHasChangesIndicator" /> class and adds all
        /// specified indicators to this instance.
        /// </summary>
        /// <param name="indicators">The list of indicators to add to this instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public CompositeHasChangesIndicator(IEnumerable<IHasChangesIndicator> indicators)
            : this()
        {
            AddRange(indicators);
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

            OnPropertyChanged(() => HasChanges);
        }

        /// <inheritdoc />
        public bool HasChanges
        {
            get { return _indicators.Any(indicator => indicator.HasChanges); }
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
        public void AddRange(params IHasChangesIndicator[] indicators)
        {
            AddRange(indicators as IEnumerable<IHasChangesIndicator>);
        }

        /// <summary>
        /// Adds all specified indicators to this indicator.
        /// </summary>
        /// <param name="indicators">The list of indicators to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indicators"/> is <c>null</c>.
        /// </exception>
        public void AddRange(IEnumerable<IHasChangesIndicator> indicators)
        {
            if (indicators == null)
            {
                throw new ArgumentNullException("indicators");
            }
            foreach (var indicator in indicators)
            {
                Add(indicator, false);
            }
            OnHasChangesChanged();
        }

        /// <summary>
        /// If not <c>null</c>, adds the specified <paramref name="indicator"/> to this indicator.
        /// </summary>
        /// <param name="indicator">The indicator to add.</param>
        /// <remarks>
        /// <paramref name="indicator"/> is only added if it not <c>null</c> and not already present.
        /// </remarks>
        public void Add(IHasChangesIndicator indicator)
        {
            Add(indicator, true);
        }

        private void Add(IHasChangesIndicator indicator, bool raiseHasChangesChanged)
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
        public void Remove(IHasChangesIndicator indicator)
        {
            Remove(indicator, true);
        }

        private void Remove(IHasChangesIndicator indicator, bool raiseHasChangesChanged)
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
            var indicators = new List<IHasChangesIndicator>(_indicators);
                       
            foreach (var indicator in indicators)
            {
                Remove(indicator, false);
            }
            OnHasChangesChanged();
        }

        #endregion
    }
}
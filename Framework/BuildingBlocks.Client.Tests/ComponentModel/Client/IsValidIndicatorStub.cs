using System;

namespace Kingo.BuildingBlocks.ComponentModel.Client
{
    internal sealed class IsValidIndicatorStub : PropertyChangedBase, INotifyIsValid
    {
        private bool _isValid;

        public IsValidIndicatorStub(bool isValid)
        {
            _isValid = isValid;
        }

        public event EventHandler IsValidChanged;

        private void OnIsValidChanged()
        {
            IsValidChanged.Raise(this);

            NotifyOfPropertyChange(() => IsValid);
        }

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;

                    OnIsValidChanged();
                }
            }
        }
    }
}

using System;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    internal sealed class IsValidIndicatorStub : PropertyChangedNotifier, IIsValidIndicator
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

            OnPropertyChanged(() => IsValid);
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

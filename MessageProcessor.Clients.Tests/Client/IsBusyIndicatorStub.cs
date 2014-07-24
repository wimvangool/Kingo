using System;

namespace YellowFlare.MessageProcessing.Client
{
    internal sealed class IsBusyIndicatorStub : PropertyChangedNotifier, IIsBusyIndicator
    {
        private bool _isBusy;

        public IsBusyIndicatorStub(bool isBusy)
        {
            _isBusy = isBusy;
        }

        public event EventHandler IsBusyChanged;

        private void OnIsBusyChanged()
        {
            IsBusyChanged.Raise(this);

            OnPropertyChanged(() => IsBusy);
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;

                    OnIsBusyChanged();
                }
            }
        }
    }
}

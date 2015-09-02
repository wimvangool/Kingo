using System;

namespace Kingo.BuildingBlocks.Messaging.Client
{
    internal sealed class IsBusyIndicatorStub : PropertyChangedBase, INotifyIsBusy
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

            NotifyOfPropertyChange(() => IsBusy);
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

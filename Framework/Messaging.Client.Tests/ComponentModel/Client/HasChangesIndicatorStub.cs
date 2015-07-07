using System;

namespace Syztem.ComponentModel.Client
{
    internal sealed class HasChangesIndicatorStub : PropertyChangedBase, INotifyHasChanges
    {
        private bool _hasChanges;

        public HasChangesIndicatorStub(bool hasChanges)
        {
            _hasChanges = hasChanges;
        }

        public event EventHandler HasChangesChanged;

        private void OnHasChangesChanged()
        {
            HasChangesChanged.Raise(this);

            NotifyOfPropertyChange(() => HasChanges);
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;

                    OnHasChangesChanged();
                }
            }
        }        
    }
}

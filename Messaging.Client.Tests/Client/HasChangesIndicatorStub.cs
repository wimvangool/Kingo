namespace System.ComponentModel.Messaging.Client
{
    internal sealed class HasChangesIndicatorStub : PropertyChangedNotifier, IHasChangesIndicator
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

            OnPropertyChanged(() => HasChanges);
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

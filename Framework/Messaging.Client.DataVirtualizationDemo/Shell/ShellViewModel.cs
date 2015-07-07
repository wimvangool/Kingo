using System.Windows.Input;
using Caliburn.Micro;
using Syztem.ComponentModel.Client.DataVirtualization;

namespace Syztem.ComponentModel.Client.Shell
{
    public sealed class ShellViewModel : Screen
    {
        private readonly RelayCommand _reloadListViewCollectionCommand;
        private readonly RelayCommand _reloadDataGridCollectionCommand;

        public ShellViewModel()
        {
            _reloadListViewCollectionCommand = new RelayCommand(() =>
            {
                ListViewCollection = new LargeIntegerCollection("ListView");
            }); 
            _reloadDataGridCollectionCommand = new RelayCommand(() =>
            {
                DataGridCollection = new LargeIntegerCollection("DataGrid");
            });
            DisplayName = "Data Virtualization Sample";
        }                

        public ICommand ReloadListViewCommand
        {
            get { return _reloadListViewCollectionCommand; }
        }

        public ICommand ReloadDataGridCommand
        {
            get { return _reloadDataGridCollectionCommand; }
        }

        private VirtualCollection<int> _listViewCollection;

        public VirtualCollection<int> ListViewCollection
        {
            get { return _listViewCollection; }
            private set
            {
                if (_listViewCollection != value)
                {
                    _listViewCollection = value;

                    NotifyOfPropertyChange(() => ListViewCollection);
                }                
            }
        }

        private VirtualCollection<int> _dataGridCollection;

        public VirtualCollection<int> DataGridCollection
        {
            get { return _dataGridCollection; }
            private set
            {
                if (_dataGridCollection != value)
                {
                    _dataGridCollection = value;

                    NotifyOfPropertyChange(() => DataGridCollection);
                }                
            }
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                if (_listViewCollection != null)
                {
                    _listViewCollection.Dispose();
                }
                if (_dataGridCollection != null)
                {
                    _dataGridCollection.Dispose();
                }
            }
            base.OnDeactivate(close);
        }
    }
}

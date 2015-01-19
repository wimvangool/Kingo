using System.ComponentModel.Client;
using System.ComponentModel.Client.DataVirtualization;
using System.Windows.Input;
using Caliburn.Micro;

namespace System.ComponentModel.WpfApplication.Shell
{
    internal sealed class ShellViewModel : Screen
    {
        private readonly RelayCommand _reloadListViewCollectionCommand;
        private readonly RelayCommand _reloadDataGridCollectionCommand;

        public ShellViewModel()
        {
            _reloadListViewCollectionCommand = new RelayCommand(() =>
            {
                ListViewCollection = new VirtualCollection<int>(new IntegerItemLoader("ListView"));
            }); 
            _reloadDataGridCollectionCommand = new RelayCommand(() =>
            {
                DataGridCollection = new VirtualCollection<int>(new IntegerItemLoader("DataGrid"));
            });  
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
    }
}

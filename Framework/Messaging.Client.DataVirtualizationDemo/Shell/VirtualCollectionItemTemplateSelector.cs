using System.ComponentModel.Client.DataVirtualization;
using System.Windows;
using System.Windows.Controls;

namespace System.ComponentModel.Client.Shell
{
    public sealed class VirtualCollectionItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NotLoadedItemTemplate
        {
            get;
            set;
        }

        public DataTemplate LoadedItemTemplate
        {
            get;
            set;
        }

        public DataTemplate FailedToLoadItemTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            IVirtualCollectionItem virtualCollectionItem;

            if (TryGetVirtualCollectionItem(item, container, out virtualCollectionItem))
            {
                if (virtualCollectionItem.IsNotLoaded)
                {
                    return NotLoadedItemTemplate;
                }
                if (virtualCollectionItem.IsLoaded)
                {
                    return LoadedItemTemplate;
                }
                return FailedToLoadItemTemplate;
            }
            return base.SelectTemplate(item, container);
        }

        private static bool TryGetVirtualCollectionItem(object item, DependencyObject container, out IVirtualCollectionItem virtualCollectionItem)
        {
            virtualCollectionItem = item as IVirtualCollectionItem;

            if (virtualCollectionItem != null)
            {
                return true;
            }
            var presenter = (ContentPresenter)container;
            var cell = (DataGridCell) presenter.Parent;

            virtualCollectionItem = cell.DataContext as IVirtualCollectionItem;

            return virtualCollectionItem != null;
        }
    }
}

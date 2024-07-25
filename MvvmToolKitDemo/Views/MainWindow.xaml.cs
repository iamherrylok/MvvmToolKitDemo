using MvvmToolKitDemo.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MvvmToolKitDemo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            var selectedItem = treeView.SelectedItem as TreeViewItem;

            if (selectedItem != null)
            {
            }
        }

        private TreeViewItem GetParentTreeViewItem(TreeViewItem item)
        {
            // Traverses the TreeViewItems to find the parent of the given item
            var parent = VisualTreeHelper.GetParent(item) as TreeViewItem;

            // Traverse up the visual tree until we find the parent TreeViewItem
            while (parent != null && !(parent.Parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent) as TreeViewItem;
            }

            return parent;
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }

    public class ImageLibraryTreeViewSelector : StyleSelector
    {
        public Style? GroupStyle { get; set; }

        public Style? ItemStyle { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            if (item is Group)
                return GroupStyle;

            if (item is Item)
                return ItemStyle;

            return base.SelectStyle(item, container);
        }
    }
}
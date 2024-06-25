using MvvmToolKitDemo.UI;
using MvvmToolKitDemo.UI.Helpers;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;

namespace MvvmToolKitDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Rename(object sender, RoutedEventArgs e)
        {
            var treeViewItem = treeListView.ItemContainerGenerator.ContainerFromItem(treeListView.SelectedItem);

            var editableTextBox = VisualHelper.GetChild<EditableTextBox>(treeViewItem!);

            if (editableTextBox is { })
                editableTextBox.InEditMode = true;
        }
    }
}
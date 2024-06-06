using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace MvvmToolKitDemo.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "MvvmToolKit";

        [ObservableProperty]
        private string _text = "DemoName";

        [ObservableProperty]
        private ObservableCollection<string> _validationNames = new() { "aaa" };

        [ObservableProperty]
        private SplitButtonViewModel? _splitButtonViewModel;

        [RelayCommand]
        private void PopupClose()
        {
            MessageBox.Show("aaaa");
        }
    }
}

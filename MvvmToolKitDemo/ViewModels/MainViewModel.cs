using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

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
    }
}

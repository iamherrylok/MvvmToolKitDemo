using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.CodeAnalysis.CSharp;
using MvvmToolKitDemo.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        [ObservableProperty]
        private ObservableCollection<Group> _groups = [];

        [ObservableProperty]
        private ObservableCollection<object> _selectedGroups = [];

        public MainViewModel()
        {
            for (int i = 0; i < 20; i++)
                _groups.Add(new Group(i.ToString()));
        }

        [RelayCommand]
        private void PopupClose()
        {
            MessageBox.Show("aaaa");
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmToolKitDemo.Models;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;

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
        private ObservableCollection<object> _selectedItems = [];

        [ObservableProperty]
        private object _selectedItem;

        [ObservableProperty]
        private string _text1 = "aaaaaaaa";

        [ObservableProperty]
        private string _text2 = "bbbbbvvvv";

        public MainViewModel()
        {
            for (int i = 0; i < 200; i++)
                _groups.Add(new Group(i.ToString()));
        }

        [RelayCommand]
        private void PopupClose()
        {
            var group = new Group("New Group");

            Groups.Insert(0, group);

            SelectedItem = group.Items.First();
        }

        [RelayCommand]
        private void Delete()
        {

            if (SelectedItem is Group group)
                Groups.Remove(group);

            if (SelectedItem is Item item)
            {
                var groupOfItem = Groups.FirstOrDefault(g => g.Items.Any(i => i.Equals(item)));
                groupOfItem?.Items.Remove(item);
            }

            if (SelectedItem is SubItem subItem)
            {
                foreach (var g in Groups)
                {
                    foreach (var i in g.Items)
                    {
                        if (i.Items.Any(si => si.Equals(subItem)))
                        {
                            i.Items.Remove(subItem);

                            SelectedItem = i.Items[0];

                            return;
                        }
                    }
                }
            }
        }

       
    }
}

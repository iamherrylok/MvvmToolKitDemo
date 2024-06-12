using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MvvmToolKitDemo.Models
{
    public partial class Group : ObservableObject
    {
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private ObservableCollection<Item> _items = [];

        public Group(string name)
        {
            _name = $"Group {name}";

            for (int i = 0; i < 20; i++)
                _items.Add(new Item(i.ToString()));
        }
    }
}

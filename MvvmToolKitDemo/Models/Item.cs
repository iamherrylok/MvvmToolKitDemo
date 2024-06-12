using CommunityToolkit.Mvvm.ComponentModel;

namespace MvvmToolKitDemo.Models
{
    public partial class Item : ObservableObject
    {
        [ObservableProperty]
        private string? _name;

        public Item(string name)
        {
            _name = $"Item {name}";
        }
    }
}

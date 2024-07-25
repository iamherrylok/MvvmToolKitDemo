using CommunityToolkit.Mvvm.ComponentModel;

namespace MvvmToolKitDemo.Models
{
    public partial class SubItem : ObservableObject
    {
        [ObservableProperty]
        private string? _name;

        public SubItem(string name)
        {
            _name = $"SubItem {name}";
        }
    }
}

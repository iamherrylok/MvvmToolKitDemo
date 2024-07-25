using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MvvmToolKitDemo.Models
{
    public partial class Group : ObservableObject, IDataErrorInfo
    {
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _display;

        [ObservableProperty]
        private ObservableCollection<Item> _items = [];

        [ObservableProperty]
        private bool _isSelected = true;

        [ObservableProperty]
        private bool _isExpanded = true;

        public Group(string name)
        {
            Name = $"Group {name}";
            Display = $"组 {name}";

            for (int i = 0; i < 200; i++)
                Items.Add(new Item(i.ToString()));
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName.Equals(nameof(Name)))
                {
                    if (Name.Equals("ABC"))
                        result = "Invalid Name";
                }

                return result;
            }
        }

        public string Error { get; set; } = default!;

        [RelayCommand]
        public void ChangeItemName()
        {
            MessageBox.Show("fdsafdsa");
        }
    }
}

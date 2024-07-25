using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MvvmToolKitDemo.Models
{
    public partial class Item : ObservableObject, IDataErrorInfo
    {
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private bool _isExpanded = true;

        [ObservableProperty]
        private ObservableCollection<SubItem> _items = [];

        public Item(string name)
        {
            _name = $"Item {name}";

            for (int i = 0; i < 20; i++)
                Items.Add(new SubItem(i.ToString()));
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
        public void ChangeName()
        {
            MessageBox.Show("dsafdsa");
        }
    }
}

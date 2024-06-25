using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MvvmToolKitDemo.Models
{
    public partial class Group : ObservableObject, IDataErrorInfo
    {
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private ObservableCollection<Item> _items = [];

        public Group(string name)
        {
            _name = $"GroupGroupGroupGroupGroupGroupGroup {name}";

            for (int i = 0; i < 20; i++)
                _items.Add(new Item(i.ToString()));
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
    }
}

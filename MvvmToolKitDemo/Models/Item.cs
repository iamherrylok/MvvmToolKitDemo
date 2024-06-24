using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;

namespace MvvmToolKitDemo.Models
{
    public partial class Item(string name) : ObservableObject, IDataErrorInfo
    {
        [ObservableProperty]
        private string? _name = $"ItemItemItemItemItemItemItemItem {name}";

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

        partial void OnNameChanged(string? oldValue, string? newValue)
        {
            Debug.WriteLine(newValue);
        }
    }
}

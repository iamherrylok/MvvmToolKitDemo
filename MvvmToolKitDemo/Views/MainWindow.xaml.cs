using MvvmToolKitDemo.UI;
using MvvmToolKitDemo.UI.Helpers;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MvvmToolKitDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var editableTextBox = VisualHelper.GetChild<EditableTextBox>(sender as DependencyObject);

            if (editableTextBox != null)
                editableTextBox.InEditMode = true;
        }
    }

    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Field cannot be empty.");
            }

            if (value.Equals("123456789"))
            {
                return new ValidationResult(false, "Cannot be 123456789");
            }

            return ValidationResult.ValidResult;
        }
    }
}
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

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
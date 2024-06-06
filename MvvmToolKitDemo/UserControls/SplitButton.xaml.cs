using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MvvmToolKitDemo.UserControls
{
    /// <summary>
    /// SplitButton.xaml 的交互逻辑
    /// </summary>
    public partial class SplitButton : UserControl
    {
        public static readonly DependencyProperty PopupTemplateProperty =
            DependencyProperty.Register(nameof(PopupTemplate), typeof(ControlTemplate), typeof(SplitButton), new PropertyMetadata(null));

        public static Popup? _popup;

        public SplitButton()
        {
            InitializeComponent();
        }

        public ControlTemplate PopupTemplate
        {
            get => (ControlTemplate)GetValue(PopupTemplateProperty);
            set => SetValue(PopupTemplateProperty, value);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_popup == null)
            {
                _popup = new Popup();
            }

            var dataTemplate = (DataTemplate)FindResource("ElementPopupTemplate");
            _popup.Child = (UIElement)dataTemplate.LoadContent();
            _popup.DataContext = DataContext;
            _popup.PlacementTarget = this;
            _popup.IsOpen = true;
            _popup.StaysOpen = false;
        }
    }
}

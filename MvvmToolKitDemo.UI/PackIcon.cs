using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MvvmToolKitDemo.UI
{
    public class PackIcon : Control
    {
        private static readonly Lazy<IReadOnlyDictionary<PackIconKind, string>> _packIconMap
            = new Lazy<IReadOnlyDictionary<PackIconKind, string>>(PackIconFactory.Create);

        public static readonly DependencyProperty KindProperty;
        public static readonly DependencyPropertyKey DataPropertyKey;
        public static readonly DependencyProperty DataProperty;

        static PackIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));

            KindProperty = DependencyProperty.Register(nameof(Kind), typeof(PackIconKind), typeof(PackIcon), new PropertyMetadata(default(PackIconKind), OnPackIconKindChanged));
            DataPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIcon), new PropertyMetadata(null));
            DataProperty = DataPropertyKey.DependencyProperty;
        }

        public PackIconKind Kind
        {
            get => (PackIconKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        [TypeConverter(typeof(GeometryConverter))]
        public string Data
        {
            get => (string)GetValue(DataProperty);
            private set => SetValue(DataPropertyKey, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateData();
        }

        private static void OnPackIconKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((PackIcon)d).UpdateData();

        private void UpdateData()
        {
            var data = string.Empty;

            _packIconMap.Value?.TryGetValue(Kind, out data);

            Data = data;
        }
    }
}

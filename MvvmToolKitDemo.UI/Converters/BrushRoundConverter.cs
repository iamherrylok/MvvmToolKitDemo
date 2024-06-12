using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MvvmToolKitDemo.UI.Converters
{
    public class BrushRoundConverter : IValueConverter
    {
        public Brush? HighValue { get; set; } = Brushes.White;

        public Brush? LowValue { get; set; } = Brushes.Black;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush solidColorBrush) return null;

            return solidColorBrush.Color.IsLightColor()
                ? HighValue
                : LowValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    public static class ColorAssist
    {
        public static bool IsLightColor(this Color color)
        {
            static double rgb_srgb(double d)
            {
                d /= 255.0;
                return (d > 0.03928)
                    ? Math.Pow((d + 0.055) / 1.055, 2.4)
                    : d / 12.92;
            }
            double r = rgb_srgb(color.R);
            double g = rgb_srgb(color.G);
            double b = rgb_srgb(color.B);

            double luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;
            return luminance > 0.179;
        }
    }
}

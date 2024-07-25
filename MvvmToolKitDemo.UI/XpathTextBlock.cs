using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MvvmToolKitDemo.UI
{
    public class XpathTextBlock : TextBlock
    {
        public static new readonly DependencyProperty TextProperty;

        static XpathTextBlock()
        {
            TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(XpathTextBlock), new FrameworkPropertyMetadata(null, OnTextChanged));
        }

        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as XpathTextBlock;
            textBlock?.XpathFormatting();
        }

        private void XpathFormatting()
        {
            Inlines.Clear();

            var regex = new Regex("%%%([^%]+?)%%%");
            var splittedTexts = regex.Split(Text);
            var nodes = regex.Matches(Text).Where(x => x.Success).Select(m => m.Groups[1].Value).ToList();

            foreach (var text in splittedTexts)
            {
                var run = new Run(text);

                if (nodes.Contains(text))
                    run.Foreground = Brushes.Blue;

                Inlines.Add(run);
            }

        }
    }
}

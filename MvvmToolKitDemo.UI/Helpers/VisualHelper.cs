using System.Windows;
using System.Windows.Media;

namespace MvvmToolKitDemo.UI.Helpers
{
    public class VisualHelper
    {
        public static T? GetChild<T>(DependencyObject d) where T : DependencyObject
        {
            if (d == null) 
                return default;

            if (d is T t) 
                return t;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);

                var result = GetChild<T>(child);
                if (result != null) 
                    return result;
            }

            return default;
        }

        public static List<T> FindVisualChild<T>(DependencyObject? parent) where T : DependencyObject
        {
            var visuals = new List<T>();
            if (parent != null)
            {
                int numVisuals = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < numVisuals; i++)
                {
                    var v = VisualTreeHelper.GetChild(parent, i) as Visual;
                    if (v is T child)
                        visuals.Add(child);

                    visuals.AddRange(FindVisualChild<T>(v));
                }
            }

            return visuals;
        }

        public static T? GetChild<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            if (obj != null)
            {
                for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);

                    if (child is T t && (t.Name == name || string.IsNullOrEmpty(name)))
                    {
                        return t;
                    }
                    else
                    {
                        // 在下一级中没有找到指定名字的子控件，就再往下一级找
                        var grandChild = GetChild<T>(child, name);
                        if (grandChild != null)
                            return grandChild;
                    }
                }
            }

            return default;
        }

        public static T? GetAncestor<T>(DependencyObject dobj, int index = 1, int maxDeep = -1, string? name = null) 
            where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dobj); 
            var findIndex = 0;
            var findDeep = 0;
            if (parent is T)
            {
                findIndex++;
            }
            while (!(parent is T && findIndex == index) && parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent is T t && (t.Name == name || string.IsNullOrEmpty(name)))
                {
                    findIndex++;
                }
                if (maxDeep != -1)
                {
                    findDeep++;
                    if (findDeep >= maxDeep)
                    {
                        break;
                    }
                }
            }

            return parent as T;
        }
    }
}

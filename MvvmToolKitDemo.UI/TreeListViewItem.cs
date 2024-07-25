using MvvmToolKitDemo.UI.Internal;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MvvmToolKitDemo.UI
{
    [TemplatePart(Name = ContentPresenterPart, Type = typeof(TreeListViewContentPresenter))]
    public class TreeListViewItem(TreeListView treeListView) : ListViewItem
    {
        internal const string ContentPresenterPart = "PART_ContentPresenter";

        public static readonly DependencyProperty IsExpandedProperty;
        public static readonly DependencyProperty HasItemsProperty;
        public static readonly DependencyProperty LevelProperty;
        public static readonly DependencyProperty DisableExpandOnDoubleClickProperty;
        internal static readonly DependencyProperty ChildrenProperty;

        static TreeListViewItem()
        {
            IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(TreeListViewItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExpandedChanged));
            HasItemsProperty = DependencyProperty.Register(nameof(HasItems), typeof(bool), typeof(TreeListViewItem), new PropertyMetadata(false));
            LevelProperty = DependencyProperty.Register(nameof(Level), typeof(int), typeof(TreeListViewItem), new PropertyMetadata(0));
            DisableExpandOnDoubleClickProperty = DependencyProperty.Register(nameof(DisableExpandOnDoubleClick), typeof(bool), typeof(TreeListViewItem), new PropertyMetadata(false));
            ChildrenProperty = DependencyProperty.Register(nameof(Children), typeof(IEnumerable), typeof(TreeListViewItem), new PropertyMetadata(null, OnChildrenChanged));
        }

        private TreeListViewContentPresenter? ContentPresenter { get; set; }

        private TreeListView? TreeListView { get; set; } = treeListView;

        public IEnumerable<object?> GetChildren() => Children?.OfType<object?>() ?? [];

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public bool HasItems
        {
            get => (bool)GetValue(HasItemsProperty);
            set => SetValue(HasItemsProperty, value);
        }

        public int Level
        {
            get => (int)GetValue(LevelProperty);
            set => SetValue(LevelProperty, value);
        }

        public bool DisableExpandOnDoubleClick
        {
            get => (bool)GetValue(DisableExpandOnDoubleClickProperty);
            set => SetValue(DisableExpandOnDoubleClickProperty, value);
        }

        internal IEnumerable? Children
        {
            get => (IEnumerable?)GetValue(ChildrenProperty);
            set => SetValue(ChildrenProperty, value);
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeListViewItem item)
            {
                item.TreeListView?.ItemExpandedChanged(item);
            }
        }

        private static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var presenter = (TreeListViewItem)d;
            presenter.OnChildrenChanged(e);
        }

        private void OnChildrenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged oldCollectionChanged)
            {
                CollectionChangedEventManager.RemoveHandler(oldCollectionChanged, CollectionChanged_CollectionChanged);
            }
            if (e.NewValue is INotifyCollectionChanged collectionChanged)
            {
                CollectionChangedEventManager.AddHandler(collectionChanged, CollectionChanged_CollectionChanged);
            }

            OnChildrenChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void CollectionChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            => OnChildrenChanged(e);

        private void OnChildrenChanged(NotifyCollectionChangedEventArgs e)
        {
            UpdateHasChildren();
            TreeListView?.ItemsChildrenChanged(this, e);
        }

        internal void PrepareTreeListViewItem(object? item, TreeListView treeListView, int level, bool isExpanded)
        {
            Level = level;
            TreeListView = treeListView;

            Dispatcher.BeginInvoke(() =>
            {
                if (GetTemplate() is HierarchicalDataTemplate { ItemsSource: { } itemsSourceBinding })
                {
                    SetBinding(ChildrenProperty, itemsSourceBinding);
                }
                IsExpanded = isExpanded;
            });

            DataTemplate? GetTemplate() 
                => 
                ContentTemplate ??
                   ContentTemplateSelector?.SelectTemplate(item, this) ??
                   ContentPresenter?.Template;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentPresenter = GetTemplateChild(ContentPresenterPart) as TreeListViewContentPresenter;

            if (ContentPresenter is { } contentPresenter)
            {
                WeakEventManager<TreeListViewContentPresenter, EventArgs>.AddHandler(
                    contentPresenter, nameof(TreeListViewContentPresenter.TemplateChanged), OnTemplateChanged);

                void OnTemplateChanged(object? sender, EventArgs e)
                {
                    PrepareTreeListViewItem(Content, TreeListView!, Level, IsExpanded);
                }
            }
        }

        internal void ClearTreeListViewItem(object _, TreeListView __)
        {
            if (Children is INotifyCollectionChanged collectionChanged)
            {
                CollectionChangedEventManager.RemoveHandler(collectionChanged, CollectionChanged_CollectionChanged);
            }
            TreeListView = null;
        }

        private void UpdateHasChildren()
        {
            SetCurrentValue(HasItemsProperty, GetChildren().Any());
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (!e.Handled && !DisableExpandOnDoubleClick && e.ChangedButton == MouseButton.Left)
            {
                IsExpanded = !IsExpanded;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        IsExpanded = true;
                        e.Handled = true;
                        break;
                    case Key.Left:
                        if (IsExpanded)
                            IsExpanded = false;
                        else
                            TreeListView?.MoveSelectionToParent(this);
                        e.Handled = true;
                        break;
                }
            }
        }
    }
}

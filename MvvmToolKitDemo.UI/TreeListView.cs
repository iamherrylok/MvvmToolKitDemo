using System.Collections.Specialized;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows;
using MvvmToolKitDemo.UI.Automation.Peers;
using MvvmToolKitDemo.UI.Internal;
using System.Collections.ObjectModel;

namespace MvvmToolKitDemo.UI
{
    public class TreeListView : ListView
    {
        public static readonly DependencyProperty LevelIndentSizeProperty;
        public static new readonly DependencyProperty SelectedItemsProperty;

        static TreeListView()
        {
            ItemsSourceProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata() { CoerceValueCallback = CoerceItemsSource });

            LevelIndentSizeProperty = DependencyProperty.Register(nameof(LevelIndentSize), typeof(double), typeof(TreeListView), new PropertyMetadata(16.0));
            SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(ObservableCollection<object>), typeof(TreeListView), new PropertyMetadata(new ObservableCollection<object>(Array.Empty<object>())));
        }


        public double LevelIndentSize
        {
            get => (double)GetValue(LevelIndentSizeProperty);
            set => SetValue(LevelIndentSizeProperty, value);
        }

        public new ObservableCollection<object> SelectedItems
        {
            get => (ObservableCollection<object>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        internal TreeListViewItemsCollection? InternalItemsSource { get; set; }

        private static object? CoerceItemsSource(DependencyObject d, object? baseValue)
        {
            if (d is TreeListView treeListView)
            {
                treeListView.InternalItemsSource = new(baseValue);

                return treeListView.InternalItemsSource;
            }
            return baseValue;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new TreeListViewAutomationPeer(this);

        protected override DependencyObject GetContainerForItemOverride()
            => new TreeListViewItem(this);

        protected override bool IsItemItsOwnContainerOverride(object? item)
            => item is TreeListViewItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object? item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is TreeListViewItem treeListViewItem)
            {
                var level = 0;
                var isExpanded = false;
                var index = ItemContainerGenerator.IndexFromContainer(treeListViewItem);
                if (index >= 0 && InternalItemsSource is { } itemsSource)
                {
                    level = itemsSource.GetLevel(index);
                    isExpanded = itemsSource.GetIsExpanded(index);
                }

                treeListViewItem.PrepareTreeListViewItem(item, this, level, isExpanded);
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is TreeListViewItem treeListViewItem)
                treeListViewItem.ClearTreeListViewItem(item, this);

            base.ClearContainerForItemOverride(element, item);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            // TODO: selectedItems should be clear or not if itemsource changed
            if (e.RemovedItems.Count > 0)
            {
                foreach (var removeItem in e.RemovedItems)
                    SelectedItems.Remove(removeItem);
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (var addItem in e.AddedItems)
                {
                    if (!SelectedItems.Contains(addItem))
                        SelectedItems.Add(addItem);
                }
            }
        }

        internal void ItemExpandedChanged(TreeListViewItem item)
        {
            if (InternalItemsSource is { } itemsSource)
            {
                var index = ItemContainerGenerator.IndexFromContainer(item);
                if (index < 0)
                    return;

                var children = item.GetChildren().ToList();
                var isExpanded = item.IsExpanded;
                itemsSource.SetIsExpanded(index, isExpanded);
                if (isExpanded)
                {
                    int parentLevel = itemsSource.GetLevel(index);
                    for (int i = 0; i < children.Count; i++)
                        itemsSource.InsertWithLevel(i + index + 1, children[i], parentLevel + 1);
                }
                else
                    itemsSource.RemoveChildrenOfOffsetAdjustedItem(index);
            }
        }

        internal void ItemsChildrenChanged(TreeListViewItem item, NotifyCollectionChangedEventArgs e)
        {
            if (item.IsExpanded && InternalItemsSource is { } itemsSource)
            {
                var index = ItemContainerGenerator.IndexFromContainer(item);
                if (index < 0)
                    return;

                var parentLevel = itemsSource.GetLevel(index);
                index++;
                int adjustedIndex;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        adjustedIndex = index + GetChildrenAndGrandChildrenCountOfPriorSiblings(itemsSource, index, e.NewStartingIndex);
                        for (int i = 0; i < e.NewItems?.Count; i++)
                        {
                            itemsSource.InsertWithLevel(e.NewStartingIndex + i + adjustedIndex, e.NewItems[i]!, parentLevel + 1);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        adjustedIndex = index + GetChildrenAndGrandChildrenCountOfPriorSiblings(itemsSource, index, e.OldStartingIndex); ;
                        for (int i = 0; i < e.OldItems?.Count; i++)
                        {
                            itemsSource.RemoveOffsetAdjustedItem(e.OldStartingIndex + adjustedIndex);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        adjustedIndex = index + GetChildrenAndGrandChildrenCountOfPriorSiblings(itemsSource, index, e.OldStartingIndex);
                        for (int i = 0; i < e.NewItems?.Count; i++)
                        {
                            itemsSource.ReplaceOffsetAdjustedItem(e.OldStartingIndex + i + adjustedIndex, e.NewItems[i]!);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        int adjustedOldIndex = index + e.OldStartingIndex + GetChildrenAndGrandChildrenCountOfPriorSiblings(itemsSource, index, e.OldStartingIndex);

                        int additionalOffset = 0;
                        if (e.OldStartingIndex < e.NewStartingIndex)
                            additionalOffset = 1;

                        int adjustedNewIndex = index + e.NewStartingIndex + GetChildrenAndGrandChildrenCountOfPriorSiblings(itemsSource, index, e.NewStartingIndex + additionalOffset);

                        itemsSource.Move(adjustedOldIndex, adjustedNewIndex);

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        index--;    // Push the index back to the parent
                        int itemLevel = itemsSource.GetLevel(index);
                        var children = item.GetChildren().ToList();

                        // Remove and re-add all of the item's children
                        itemsSource.RemoveChildrenOfOffsetAdjustedItem(index);
                        index++;    // We push the index forward by 1 to be on the first element of the item's children
                        for (int i = 0; i < children.Count; i++)
                        {
                            itemsSource.InsertWithLevel(i + index, children[i], itemLevel + 1);
                        }
                        break;
                }
            }

            static int GetChildrenAndGrandChildrenCountOfPriorSiblings(TreeListViewItemsCollection collection, int startingIndex, int expectedPriorSiblingCount)
            {
                int childrenAndGrandChildrenCount = 0;
                int index = 0;
                int siblingCount = 0;

                // Determine the level expected of siblings (used for comparison)
                int siblingLevel = collection.GetLevel(startingIndex - 1) + 1;

                // Iterate while we haven't:
                //  - Exceeded the expected number of prior siblings, or
                //  - Reached the end of the InternalItemsSource, or
                //  - Reached an item with a level less than the sibling level
                while (siblingCount <= expectedPriorSiblingCount)
                {
                    // Bail out if we've reached the end of the itemsSource
                    if (startingIndex + index >= collection.Count)
                        break;

                    // Bail out if we've reached an item with a level less than the sibling level
                    int level = collection.GetLevel(startingIndex + index);
                    if (level < siblingLevel)
                        break;

                    if (level == siblingLevel)
                    {
                        siblingCount++;
                    }
                    else
                    {
                        childrenAndGrandChildrenCount++;
                    }
                    index++;
                }
                return childrenAndGrandChildrenCount;
            }
        }

        internal void MoveSelectionToParent(TreeListViewItem item)
        {
            if ((IsKeyboardFocused || IsKeyboardFocusWithin)
                && InternalItemsSource is { } itemsSource)
            {
                var index = ItemContainerGenerator.IndexFromContainer(item);
                if (index < 0) return;
                var itemLevel = itemsSource.GetLevel(index);
                for (var i = index; i > 0; i--)
                {
                    if (itemsSource.GetLevel(i) == itemLevel - 1)
                    {
                        SetSelectedItems(new[] { itemsSource[i] });
                        if (ItemContainerGenerator.ContainerFromIndex(i) is TreeListViewItem container)
                            container.Focus();

                        break;
                    }
                }
            }
        }

        private List<object?> GetExpandedChildrenAndGrandChildren(object? dataItem)
        {
            List<object?> expandedChildren = new();
            if (dataItem is null || ItemContainerGenerator.ContainerFromItem(dataItem) is not TreeListViewItem { IsExpanded: true } container)
                return expandedChildren;

            expandedChildren.Add(dataItem);

            foreach (object? grandChild in container.GetChildren())
                expandedChildren.AddRange(GetExpandedChildrenAndGrandChildren(grandChild));

            return expandedChildren;
        }

        //public object? GetParent(object? item)
        //{
        //    if (InternalItemsSource is { } itemSource &&
        //        ItemContainerGenerator.ContainerFromItem(item) is { } container &&
        //        ItemContainerGenerator.IndexFromContainer(container) is var index &&
        //        index >= 0)
        //    {
        //        return itemSource.GetParent(index);
        //    }
        //    return null;
        //}
    }
}

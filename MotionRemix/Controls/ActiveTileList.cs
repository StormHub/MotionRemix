using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MotionRemix
{
    /// <summary>
    /// A items container that presents one child item as a time.
    /// </summary>
    public class ActiveTileList : ItemsControl
    {
        #region Fields

        private ItemCollection currentItemCollection;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Static meta data registrations.
        /// </summary>
        static ActiveTileList()
        {
            Type ownerType = typeof(ActiveTileList);

            DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));

            // Command registrations
            CommandManager.RegisterClassCommandBinding(ownerType,
                new CommandBinding(PageLeftCommand, OnScrollCommand, OnQueryScrollCommand));
            CommandManager.RegisterClassCommandBinding(ownerType,
                new CommandBinding(PageRightCommand, OnScrollCommand, OnQueryScrollCommand));

        }

        #endregion Constructors

        #region Routed Commands

        private const string PageLeftCommandName = "PageLeft";

        /// <summary>
        /// <see cref="RoutedCommand"/> to scroll left.
        /// </summary>
        public static readonly RoutedCommand PageLeftCommand =
            new RoutedCommand(PageLeftCommandName, typeof(ActiveTileList));

        private const string PageRightCommandName = "PageRight";

        /// <summary>
        /// <see cref="RoutedCommand"/> to scroll right.
        /// </summary>
        public static readonly RoutedCommand PageRightCommand =
            new RoutedCommand(PageRightCommandName, typeof(ActiveTileList));

        #endregion Routed Commands

        #region Dependency Properties

        #region ItemsCount

        private const string ItemCountPropertyName = "ItemCount";

        /// <summary>
        /// Dependency property key for read only ItemCount.
        /// </summary>
        private static readonly DependencyPropertyKey ItemCountPropertyKey =
            DependencyProperty.RegisterReadOnly(
                ItemCountPropertyName,
                typeof(int),
                typeof(ActiveTileList),
                new PropertyMetadata(0));

        /// <summary>
        /// Dependency property for ItemCount.
        /// </summary>
        public static readonly DependencyProperty ItemCountProperty = ItemCountPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the item count.
        /// </summary>
        public int ItemCount
        {
            get
            {
                return (int)GetValue(ItemCountProperty);
            }
            private set
            {
                SetValue(ItemCountPropertyKey, value);
            }
        }

        #endregion ItemsCount

        #region CurrentItemIndex

        private const string CurrentItemIndexPropertyName = "CurrentItemIndex";

        /// <summary>
        /// Dependency property for CurrentItemIndex.
        /// </summary>
        public static readonly DependencyProperty CurrentItemIndexProperty =
            DependencyProperty.Register(
                CurrentItemIndexPropertyName,
                typeof(int),
                typeof(ActiveTileList),
                new PropertyMetadata(
                    0,
                    OnCurrentItemIndexPropertychanged));

        /// <summary>
        /// Gets or sets the CurrentItemIndex.
        /// </summary>
        public int CurrentItemIndex
        {
            get
            {
                return (int)GetValue(CurrentItemIndexProperty);
            }
            set
            {
                SetValue(CurrentItemIndexProperty, value);
            }
        }

        private static void OnCurrentItemIndexPropertychanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ActiveTileList target = element as ActiveTileList;
            if (target == null)
            {
                return;
            }

            target.RefreshActiveStatus();
        }

        #endregion CurrentItemIndex

        #endregion Dependency Properties

        #region Scroll

        private void ScrollLeft()
        {
            if (CurrentItemIndex > 0 && ItemCount > 0)
            {
                CurrentItemIndex -= 1;
            }
        }

        private void ScrollRight()
        {
            if (CurrentItemIndex < (ItemCount - 1) && ItemCount > 0)
            {
                CurrentItemIndex += 1;
            }
        }

        private static void OnScrollCommand(object target, ExecutedRoutedEventArgs args)
        {
            ActiveTileList control = target as ActiveTileList;
            if (control == null)
            {
                return;
            }

            if (args.Command == PageLeftCommand)
            {
                control.ScrollLeft();
            }
            else if (args.Command == PageRightCommand)
            {
                control.ScrollRight();
            }
        }

        private static void OnQueryScrollCommand(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
            ActiveTileList control = target as ActiveTileList;
            if (control == null)
            {
                return;
            }

            if (args.Command == PageLeftCommand)
            {
                args.CanExecute = (control.ItemCount > 0) && (control.CurrentItemIndex > 0);
            }
            else if (args.Command == PageRightCommand)
            {
                args.CanExecute = (control.ItemCount > 0) && (control.CurrentItemIndex < (control.ItemCount - 1));
            }
            args.ContinueRouting = true;
            args.Handled = true;
        }

        #endregion Scroll

        #region Overrides

        /// <summary>
        /// Called when the ItemsSource property changes.
        /// </summary>
        /// <param name="oldValue">
        /// Old value of the ItemsSource property.
        /// </param>
        /// <param name="newValue">
        /// New value of the ItemsSource property.
        /// </param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            RefreshCurrentItemCollection();
        }

        /// <summary>
        /// Invoked when the Items property changes.
        /// </summary>
        /// <param name="e">
        /// Information about the change.
        /// </param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            RefreshCurrentItemCollection();
        }

        #endregion Overrides

        #region Methods

        private void RefreshCurrentItemCollection()
        {
            ItemCollection itemCollection = Items;
            if (!ReferenceEquals(itemCollection, currentItemCollection))
            {
                currentItemCollection = itemCollection;
            }

            RefreshActiveStatus();
        }

        private void RefreshActiveStatus()
        {
            if (currentItemCollection.Count != ItemCount)
            {
                ItemCount = currentItemCollection.Count;
            }
            if (CurrentItemIndex >= ItemCount)
            {
                CurrentItemIndex = (ItemCount - 1);
            }
            else if (CurrentItemIndex == -1 && ItemCount > 0)
            {
                CurrentItemIndex = 0;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        #endregion Methods
    }
}

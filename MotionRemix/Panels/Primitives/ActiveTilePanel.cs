using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MotionRemix.Panels.Primitives
{
    /// <summary>
    /// A panel only presents one child at a time filled in the entire available 
    /// space and lay out all child items horizontal.
    /// </summary>
    public class ActiveTilePanel : Panel
    {
        #region Fields

        private Size lastVisualParentSize;
        private FrameworkElement visualParent;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ActiveTilePanel"/>
        /// </summary>
        public ActiveTilePanel()
        {
            LayoutUpdated += OnLayoutUpdated;
        }

        #endregion Constructors

        #region Measure And Layout

        /// <summary>
        /// Measures all child sizes for the specified <see cref="Size"/>.
        /// </summary>
        /// <param name="availableSize">
        /// The available size.
        /// </param>
        /// <returns>
        /// The measured size for all child items.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0;
            double height = 0;
            int childCount = InternalChildren.Count;
            for (int i = 0; i < childCount; i++)
            {
                UIElement element = InternalChildren[i];
                if (element != null)
                {
                    element.Measure(availableSize);
                    width = Math.Max(width, element.DesiredSize.Width);
                    height = Math.Max(height, element.DesiredSize.Height);
                }
            }

            Size size = new Size();

            // If either width or height is infinite, it means all child items
            // can fit into the panel
            if (double.IsInfinity(availableSize.Width)
                || double.IsInfinity(availableSize.Height))
            {
                size.Width = width * childCount;
                size.Height = height;
            }
            else
            {
                // All child items are as big as the panel size
                size.Width = availableSize.Width * childCount;
                size.Height = availableSize.Height;
            }

            return size;
        }

        /// <summary>
        /// Arranges the size for the specified available <see cref="Size"/>.
        /// </summary>
        /// <param name="finalSize">
        /// The final <see cref="Size"/> of the  panel.
        /// </param>
        /// <returns>
        /// The arranged size for all child items.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            lastVisualParentSize = visualParent != null ? visualParent.RenderSize : finalSize;

            int childCount = InternalChildren.Count;
            for (int i = 0; i < childCount; i++)
            {
                UIElement element = InternalChildren[i];
                Rect rect = new Rect(new Point(lastVisualParentSize.Width * i, 0), lastVisualParentSize);
                element.Arrange(rect);
            }

            return new Size(lastVisualParentSize.Width * childCount, lastVisualParentSize.Height);
        }

        #endregion Measure And Layout

        #region Overrides

        /// <summary>
        /// Returns a geometry for a clipping mask. The mask applies if the layout system attempts to 
        /// arrange an element that is larger than the available display space.
        /// </summary>
        /// <param name="layoutSlotSize">
        /// The size of the part of the element that does visual presentation. 
        /// </param>
        /// <returns>
        /// The clipping geometry.
        /// </returns>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }

        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed.
        /// </summary>
        /// <param name="oldParent">
        /// The old parent element. May be null to indicate that the element did not have a visual 
        /// parent previously.
        /// </param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            visualParent = VisualParent as FrameworkElement;
        }

        #endregion Overrides

        #region Event Handling

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (visualParent == null)
            {
                return;
            }

            if (visualParent.RenderSize != lastVisualParentSize)
            {
                InvalidateArrange();
            }
        }

        #endregion Event Handling
    }
}

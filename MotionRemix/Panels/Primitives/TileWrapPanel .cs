using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using MotionRemix.Common;

namespace MotionRemix.Panels.Primitives
{
	/// <summary>
	/// Provides tile wrap panel with animation support.
	/// </summary>
	public class TileWrapPanel : AnimationPanel
	{
		#region Fields

		private DoubleAnimation itemFadeInAnimation;
		private bool isArrangeInitialized;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Intializes a new instance of <see cref="TileWrapPanel"/>.
		/// </summary>
		public TileWrapPanel()
		{
			isArrangeInitialized = false;
		}

		#endregion Constructors

		#region Dependency Properties

		#region ItemHeight

		private const string ItemHeightPropertyName = "ItemHeight";

		/// <summary>
		/// Dependency property for ItemHeight.
		/// </summary>
		public static readonly DependencyProperty ItemHeightProperty =
			DependencyProperty.Register(
				ItemHeightPropertyName,
				typeof(double),
				typeof(TileWrapPanel),
				new FrameworkPropertyMetadata(
					50d,
					FrameworkPropertyMetadataOptions.AffectsMeasure),
				IsValidSize);

		/// <summary>
		/// ItemHeight property
		/// </summary>
		public double ItemHeight
		{
			get
			{
				return (double)GetValue(ItemHeightProperty);
			}
			set
			{
				SetValue(ItemHeightProperty, value);
			}
		}

		private static bool IsValidSize(object value)
		{
			double target = (double)value;
			return target.IsValidSizeValue();
		}

		#endregion ItemHeight

		#region ItemWidth

		private const string ItemWidthPropertyName = "ItemWidth";

		/// <summary>
		/// Dependency property for ItemWidth.
		/// </summary>
		public static readonly DependencyProperty ItemWidthProperty =
			DependencyProperty.Register(
				ItemWidthPropertyName,
				typeof(double),
				typeof(TileWrapPanel),
				new FrameworkPropertyMetadata(
					50d,
					FrameworkPropertyMetadataOptions.AffectsMeasure),
				IsValidSize);

		/// <summary>
		/// ItemWidth property
		/// </summary>
		public double ItemWidth
		{
			get
			{
				return (double)GetValue(ItemWidthProperty);
			}
			set
			{
				SetValue(ItemWidthProperty, value);
			}
		}

		#endregion ItemWidth

		#endregion Dependency Properties

		#region Overrides

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
			Size size = new Size();

			UIElement[] elements = InternalChildren
				.Cast<UIElement>()
				.Where(element => element.Visibility != Visibility.Collapsed)
				.ToArray();

			Size itemSize = new Size(ItemWidth, ItemHeight);
			foreach (UIElement child in elements)
			{
				child.Measure(itemSize);

				double width = size.Width + Math.Min(ItemWidth, child.DesiredSize.Width);
				if (width.IsGreaterThanOrCloseTo(availableSize.Width))
				{
					size.Width = availableSize.Width;
					size.Height += Math.Min(ItemHeight, child.DesiredSize.Height);
				}
				else
				{
					size.Width = width;
					size.Height = Math.Min(ItemHeight, child.DesiredSize.Height);
				}
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
			Size itemSize = new Size(ItemWidth, ItemHeight);

			UIElement[] elements = InternalChildren
				.Cast<UIElement>()
				.Where(element => element.Visibility != Visibility.Collapsed)
				.ToArray();

			int childrenPerRow = Math.Max(1, (int)Math.Floor(finalSize.Width / ItemWidth));
			for (int i = 0; i < elements.Length; i++)
			{
				Point newOffset = CalculateChildOffset(i, childrenPerRow, finalSize.Width, Children.Count);
				ArrangeElement(elements[i], new Rect(newOffset, itemSize));
			}

			isArrangeInitialized = true;

			return finalSize;
		}

		#endregion Overrides

		#region Methods

		/// <summary>
		/// Invoked when a new <see cref="UIElement"/> is added.
		/// </summary>
		/// <param name="element">
		/// The <see cref="UIElement"/> to add.
		/// </param>
		/// <param name="bounds">
		/// The element <see cref="Rect"/> bound.
		/// </param>
		/// <returns>
		/// The element location.
		/// </returns>
		protected override Point OnElementAdded(UIElement element, Rect bounds)
		{
			Point startLocation = bounds.Location;

			if (!isArrangeInitialized)
			{
				return bounds.Location;
			}

			if (itemFadeInAnimation == null)
			{
				itemFadeInAnimation =
					new DoubleAnimation
					{
						From = 0,
						Duration = new Duration(TimeSpan.FromSeconds(.5))
					};
				itemFadeInAnimation.Freeze();
			}

			element.BeginAnimation(OpacityProperty, itemFadeInAnimation);
			startLocation -= new Vector(bounds.Width, 0);

			return startLocation;
		}

		private Point CalculateChildOffset(int index, int childrenPerRow, double width, int total)
		{
			double fudge = 0;
			if (total > childrenPerRow)
			{
				fudge = (width - childrenPerRow * ItemWidth) / childrenPerRow;
			}

			int row = index / childrenPerRow;
			int column = index % childrenPerRow;
			return new Point(.5 * fudge + column * (ItemWidth + fudge), row * ItemHeight);
		}

		#endregion Methods
	}
}

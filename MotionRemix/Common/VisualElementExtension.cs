using System;
using System.Windows;
using System.Windows.Media;

namespace MotionRemix.Common
{
	/// <summary>
	/// Provides <see cref="UIElement"/> related extension methods.
	/// </summary>
	public static class VisualElementExtension
	{
		/// <summary>
		/// Get the root <see cref="UIElement"/> from the specified <see cref="DependencyObject"/>.
		/// </summary>
		/// <param name="element">
		/// The <see cref="DependencyObject"/> to get root for.
		/// </param>
		/// <returns>
		/// The root <see cref="UIElement"/> from the specified <paramref name="element"/>.
		/// </returns>
		public static UIElement RootElement(this DependencyObject element)
		{
			if (element == null)
			{
				return null;
			}

			DependencyObject reference = element;
			for (DependencyObject dependencyObject = reference; dependencyObject != null;
				dependencyObject = VisualTreeHelper.GetParent(reference))
			{
				reference = dependencyObject;
			}

			return reference as UIElement;
		}

		/// <summary>
		/// Finds the parent of the given type for the specified <see cref="DependencyObject"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The parent type to find.
		/// </typeparam>
		/// <param name="element">
		/// The <see cref="DependencyObject"/> to look for from.
		/// </param>
		/// <returns>
		/// The parent of the given type for the specified <see cref="DependencyObject"/>. If
		/// the parent does not exist, null will be returned.
		/// </returns>
		public static T FindParent<T>(this DependencyObject element) where T : DependencyObject
		{
			if (element == null)
			{
				return null;
			}

			DependencyObject parent = VisualTreeHelper.GetParent(element);
			do
			{
				T matchedParent = parent as T;
				if (matchedParent != null)
				{
					return matchedParent;
				}

				parent = VisualTreeHelper.GetParent(parent);
			}
			while (parent != null);

			return null;
		}

		/// <summary>
		/// Indicates whether the parent window for the specified <see cref="UIElement"/> is in full 
		/// screen mode or not.
		/// </summary>
		/// <param name="element">
		/// The <see cref="UIElement"/> to get parent window from.
		/// </param>
		/// <returns>
		/// True if the parent window for the specified <see cref="UIElement"/> is in full screen mode.
		/// Otherwise false.
		/// </returns>
		public static bool IsParentWindowInFullScreenMode(this UIElement element)
		{
			if (element == null)
			{
				return false;
			}

			Window window = element.RootElement() as Window;
			return window.IsInFullScreenMode();
		}

		/// <summary>
		/// Indicates whether the specified <see cref="Window"/> is in full screen mode or not.
		/// </summary>
		/// <param name="window">
		/// The <see cref="Window"/> to check for.
		/// </param>
		/// <returns>
		/// True if the specified <see cref="Window"/> is in full screen mode. Otherwise false.
		/// </returns>
		public static bool IsInFullScreenMode(this Window window)
		{
			return window != null
				&& window.ResizeMode == ResizeMode.NoResize
				&& window.WindowStyle == WindowStyle.None
				&& window.WindowState == WindowState.Maximized;
		}

		/// <summary>
		/// Switchs the specified <see cref="Window"/> into full screen mode.
		/// </summary>
		/// <param name="window">
		/// The <see cref="Window"/> to switch to full screen mode.
		/// </param>
		public static void SwitchToFullScreenMode(this Window window)
		{
			if (window == null || window.IsInFullScreenMode())
			{
				return;
			}

			window.ResizeMode = ResizeMode.NoResize;
			window.WindowStyle = WindowStyle.None;
			window.WindowState = WindowState.Maximized;
		}

		/// <summary>
		/// Executes the given action for all children of the specified element.
		/// </summary>
		/// <param name="element">
		/// The <see cref="DependencyObject"/> to look for.
		/// </param>
		/// <param name="action">
		/// The action to execute.
		/// </param>
		public static void ForAllChildren(this DependencyObject element, Action<DependencyObject> action)
		{
			if (element == null || action == null)
			{
				return;
			}

			int count = VisualTreeHelper.GetChildrenCount(element);
			for (int i = 0; i < count; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(element, i);
				action(child);
				child.ForAllChildren(action);
			}
		}

		/// <summary>
		/// Get the <see cref="Rect"/> visual bounds with render transform for the
		/// specified <see cref="UIElement"/>.
		/// </summary>
		/// <param name="element">
		/// The visual <see cref="UIElement"/>.
		/// </param>
		/// <returns>
		/// The <see cref="Rect"/> visual bounds with render transform for the specified 
		/// <see cref="UIElement"/>.
		/// </returns>
		public static Rect RenderVisualBounds(this UIElement element)
		{
			if (element == null)
			{
				return Rect.Empty;
			}

			// Visual bounds
			Rect rect = VisualTreeHelper.GetDescendantBounds(element);
			Transform transform = element.RenderTransform;
			if (rect.IsEmpty)
			{
				return rect;
			}

			if (transform == null)
			{
				return rect;
			}

			Rect bounds = transform.TransformBounds(rect);
			return bounds;
		}

		/// <summary>
		/// Recursive deep copy of the specified transform.
		/// </summary>
		/// <param name="transform">
		/// The transform to clone.
		/// </param>
		/// <returns>
		/// A deep copy of the specified transform, or null if the specified transform is null.
		/// </returns>
		public static Transform CloneTransform(this Transform transform)
		{
			if (transform == null)
			{
				return null;
			}

			ScaleTransform scaleTransform = transform as ScaleTransform;
			if (scaleTransform != null)
			{
				return new ScaleTransform
				{
					CenterX = scaleTransform.CenterX,
					CenterY = scaleTransform.CenterY,
					ScaleX = scaleTransform.ScaleX,
					ScaleY = scaleTransform.ScaleY
				};
			}

			RotateTransform rotateTransform = transform as RotateTransform;
			if (rotateTransform != null)
			{
				return new RotateTransform
				{
					Angle = rotateTransform.Angle,
					CenterX = rotateTransform.CenterX,
					CenterY = rotateTransform.CenterY
				};
			}

			SkewTransform skewTransform = transform as SkewTransform;
			if (skewTransform != null)
			{
				return new SkewTransform
				{
					AngleX = skewTransform.AngleX,
					AngleY = skewTransform.AngleY,
					CenterX = skewTransform.CenterX,
					CenterY = skewTransform.CenterY
				};
			}

			TranslateTransform translateTransform = transform as TranslateTransform;
			if (translateTransform != null)
			{
				return new TranslateTransform
				{
					X = translateTransform.X,
					Y = translateTransform.Y
				};
			}

			MatrixTransform matrixTransform = transform as MatrixTransform;
			if (matrixTransform != null)
			{
				return new MatrixTransform
				{
					Matrix = matrixTransform.Matrix
				};
			}

			TransformGroup transformGroup = transform as TransformGroup;
			if (transformGroup == null)
			{
				return null;
			}

			TransformGroup group = new TransformGroup();
			foreach (Transform childTransform in transformGroup.Children)
			{
				group.Children.Add(childTransform.CloneTransform());
			}

			return group;
		}
	}
}

using System.Windows;
using System.Windows.Media;

using MotionRemix.Common;

namespace MotionRemix
{
	/// <summary>
	/// Provides linear gradient brush offset animation for all gradient stops.
	/// </summary>
	public class GradientBrushOffsetAnimation : GradientBrushAnimationBase
	{
		#region Fields

		private const int MinimumValue = 0;  // Minimum range for start
		private const int MaximumValue = 1;  // Maximum range for end

		private double distance;

		#endregion Fields

		#region Dependency Properties

		#region StartOffset

		private const string StartOffsetPropertyName = "StartOffset";
		private const double DefaultStartOffset = 0d;

		/// <summary>
		/// Dependency property for Start offset
		/// </summary>
		public static readonly DependencyProperty StartOffsetProperty =
			DependencyProperty.Register(
				StartOffsetPropertyName,
				typeof(double),
				typeof(GradientBrushOffsetAnimation),
				new FrameworkPropertyMetadata(DefaultStartOffset),
				IsValidStartOffset);

		/// <summary>
		/// Start Offset property
		/// </summary>
		public double StartOffset
		{
			get
			{
				return (double)GetValue(StartOffsetProperty);
			}
			set
			{
				SetValue(StartOffsetProperty, value);
			}
		}

		private static bool IsValidStartOffset(object value)
		{
			double target = (double)value;

			return target.IsFiniteValue() 
				&& target.IsGreaterThanOrCloseTo(0) 
				&& target.IsLessThanOrCloseTo(1);
		}

		#endregion StartOffset

		#endregion Dependency Properties

		#region Overrides

		/// <summary>
		/// Get the current <see cref="GradientBrush"/> value for the specified progress between 0 to 1 inclusive.
		/// </summary>
		/// <param name="defaultOriginalBrush">
		/// The default <see cref="GradientBrush"/> instance.
		/// </param>
		/// <param name="defaultDestinationBrush">
		/// The default <see cref="GradientBrush"/> instance.
		/// </param>
		/// <param name="progress">
		/// The current progress between 0 to 1 inclusive.
		/// </param>
		/// <returns>
		/// The current <see cref="GradientBrush"/> value for the specified progress between 0 to 1 inclusive.
		/// </returns>
		protected override GradientBrush GetCurrentValue(
			GradientBrush defaultOriginalBrush, 
			GradientBrush defaultDestinationBrush, 
			double progress)
		{
			if (AnimationBrush == null)
			{
				return defaultOriginalBrush;
			}

			//
			//  Either no gradient stop defined or the current value is right at
			//  the start.
			//
			if (progress.IsCloseTo(MinimumValue)
				|| distance.IsCloseTo(MinimumValue))
			{
				return AnimationBrush;
			}

			GradientBrush brush = AnimationBrush.Clone();
			brush.GradientStops.Clear();

			double startOffset = StartOffset;
			double offset = distance * progress;
			foreach (GradientStop gradientStop in AnimationBrush.GradientStops)
			{
				double stepOffset = gradientStop.Offset + offset - startOffset;
				if (stepOffset.IsLessThan(0))
				{
					continue;
				}

				// Stop shifting gradient stops further beyond maximum
				if (stepOffset.IsCloseTo(MaximumValue))
				{
					break;
				}

				GradientStop step = new GradientStop(gradientStop.Color, gradientStop.Offset + offset);
				brush.GradientStops.Add(step);
			}

			brush.Freeze();

			return brush;
		}

		/// <summary>
		/// Called when current freezed copy of the <see cref="GradientBrush"/> is changed.
		/// </summary>
		/// <param name="oldValue">
		/// The original <see cref="GradientBrush"/> value.
		/// </param>
        protected override void OnAnimationBrushChanged(GradientBrush oldValue)
		{
			distance = MinimumValue;

			if (AnimationBrush != null
				&& AnimationBrush.GradientStops != null
				&& AnimationBrush.GradientStops.Count > 0)
			{
				// Measure the distance from the first offset to the maximum
				distance = (MaximumValue - AnimationBrush.GradientStops[0].Offset)
					.Abs()
					.AtMost(MaximumValue);
			}
		}

		/// <summary>
		/// Create new instance of <see cref="GradientBrushOffsetAnimation"/>.
		/// </summary>
		/// <returns>
		/// The new instance of <see cref="GradientBrushOffsetAnimation"/>.
		/// </returns>
		protected override Freezable CreateInstanceCore()
		{
			return new GradientBrushOffsetAnimation();
		}

		#endregion Overrides
	}
}

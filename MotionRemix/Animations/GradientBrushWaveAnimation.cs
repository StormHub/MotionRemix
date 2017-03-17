using System;
using System.Windows;
using System.Windows.Media;

using MotionRemix.Common;

namespace MotionRemix
{
    /// <summary>
    /// Provides sine wave propagation for gradient offsets.
    /// </summary>
    public class GradientBrushWaveAnimation : GradientBrushAnimationBase
    {
        #region Fields

        private GradientBrush brush;
        private double maximumOffset;
        private int repeat;
        private double total;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="GradientBrushWaveAnimation"/>
		/// </summary>
        public GradientBrushWaveAnimation()
		{
            brush = null;
            maximumOffset = DefaultMaximumOffset;
            repeat = DefaultRepeat;
            total = (Math.PI * 2) * repeat;
        }

		#endregion Constructors

        #region Dependency Properties

        #region Repeat Property

        private const string RepeatPropertyName = "Repeat";
        private const int DefaultRepeat = 4;

        /// <summary>
        /// Dependency property for Repeat.
        /// </summary>
        public static readonly DependencyProperty RepeatProperty =
            DependencyProperty.Register(
                RepeatPropertyName,
                typeof(int),
                typeof(GradientBrushWaveAnimation),
                new FrameworkPropertyMetadata(
                    DefaultRepeat,
                    RepeatPropertyChanged),
                IsValidRepeat);

        /// <summary>
        /// Gets or sets the repeat of the <see cref="GradientBrushWaveAnimation"/>.
        /// </summary>
        /// <returns>
        /// The repeat of the <see cref="GradientBrushWaveAnimation"/>.
        /// </returns>
        public int Repeat
        {
            get
            {
                return (int)GetValue(RepeatProperty);
            }
            set
            {
                SetValue(RepeatProperty, value);
            }
        }

        private static void RepeatPropertyChanged(
            DependencyObject element, 
            DependencyPropertyChangedEventArgs args)
        {
            GradientBrushWaveAnimation target = element as GradientBrushWaveAnimation;
            if (target == null)
            {
                return;
            }

            int oldValue = (int)args.OldValue;
            int newValue = (int)args.NewValue;

            target.OnRepeatPropertyChanged(oldValue, newValue);
        }

        private static bool IsValidRepeat(object value)
        {
            int target = (int)value;
            return target > 0;
        }

        #endregion Repeat Property

        #region MaximumOffset Property

        private const string MaximumOffsetPropertyName = "MaximumOffset";
        private const double DefaultMaximumOffset = 0.04d;

        /// <summary>
        /// Dependency property for MaximumOffset.
        /// </summary>
        public static readonly DependencyProperty MaximumOffsetProperty =
            DependencyProperty.Register(
                MaximumOffsetPropertyName,
                typeof(double),
                typeof(GradientBrushWaveAnimation),
                new FrameworkPropertyMetadata(
                    DefaultMaximumOffset,
                    MaximumOffsetPropertyChanged),
                IsValidMaximumOffset);

        /// <summary>
        /// Gets or sets the maximum offset of the <see cref="GradientBrushWaveAnimation"/>.
        /// </summary>
        /// <returns>
        /// The maximum offset of the <see cref="GradientBrushWaveAnimation"/>.
        /// </returns>
        public double MaximumOffset
        {
            get
            {
                return (double)GetValue(MaximumOffsetProperty);
            }
            set
            {
                SetValue(MaximumOffsetProperty, value);
            }
        }

        private static void MaximumOffsetPropertyChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            GradientBrushWaveAnimation target = element as GradientBrushWaveAnimation;
            if (target == null)
            {
                return;
            }

            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;

            target.OnMaximumOffsetPropertyChanged(oldValue, newValue);
        }

        private static bool IsValidMaximumOffset(object value)
        {
            double target = (double)value;
            return target.IsBetween(0, 0.5);
        }

        #endregion WaveLength Property

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
            if (AnimationBrush == null
                || brush == null)
            {
                return defaultDestinationBrush;
            }

            double delta = maximumOffset * (total * progress).Cos();
            int k = (int)(repeat * progress).Floor();

            for (int i = 0; i < AnimationBrush.GradientStops.Count; i++)
            {
                double offset = AnimationBrush.GradientStops[i].Offset;
                int p = (int)(offset * repeat).Floor();
                if (k == p)
                {
                    brush.GradientStops[i].Offset = (offset - delta).Clamp(0, 1);
                }
            }

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
            brush = null;

            if (AnimationBrush == null
                || AnimationBrush.GradientStops == null
                || AnimationBrush.GradientStops.Count == 0)
            {
                return;
            }

            brush = AnimationBrush.Clone();
        }

        /// <summary>
        /// Create new instance of <see cref="GradientBrushWaveAnimation"/>.
        /// </summary>
        /// <returns>
        /// The new instance of <see cref="GradientBrushWaveAnimation"/>.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GradientBrushWaveAnimation();
        }

        #endregion Overrides

        #region Property Changes

        private void OnRepeatPropertyChanged(int oldValue, int newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            repeat = newValue;
            total = Math.PI * 2 * repeat;
        }

        private void OnMaximumOffsetPropertyChanged(double oldValue, double newValue)
        {
            if (oldValue.IsCloseTo(newValue))
            {
                return;
            }

            maximumOffset = newValue;
        }

        #endregion Property Changes
    }
}

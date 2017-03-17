using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using MotionRemix.Common;

namespace MotionRemix
{
	/// <summary>
	/// Base class for all <see cref="GradientBrush"/> related animations.
	/// </summary>
	public abstract class GradientBrushAnimationBase : AnimationTimeline
	{
		#region Fields

		private GradientBrush gradientBrush;

		#endregion Fields

		#region Dependency Properties

		#region EasingFunction

		private const string EasingFunctionPropertyName = "EasingFunction";

		/// <summary>
		/// Dependency property for EasingFunction
		/// </summary>
		public static readonly DependencyProperty EasingFunctionProperty =
			DependencyProperty.Register(
				EasingFunctionPropertyName,
				typeof(IEasingFunction),
				typeof(GradientBrushAnimationBase));

		/// <summary>
		/// Easing function property.
		/// </summary>
		public IEasingFunction EasingFunction
		{
			get
			{
				return (IEasingFunction)GetValue(EasingFunctionProperty);
			}
			set
			{
				SetValue(EasingFunctionProperty, value);
			}
		}

		#endregion EasingFunction

		#endregion Dependency Properties

		#region Overrides

		/// <summary>
		/// Gets the target property type.
		/// </summary>
		public override Type TargetPropertyType
		{
			get
			{
				ReadPreamble();
				return typeof(GradientBrush);
			}
		}

		/// <summary>
		/// Gets the current value of the animation.
		/// </summary>
		/// <param name="defaultOriginValue">
		/// The origin value provided to the animation if the animation does not have
		/// its own start value. If this animation is the first in a composition chain
		/// it will be the base value of the property being animated; otherwise it will
		/// be the value returned by the previous animation in the chain.
		/// </param>
		/// <param name="defaultDestinationValue">
		/// The destination value provided to the animation if the animation does not
		/// have its own destination value.
		/// </param>
		/// <param name="animationClock">
		/// The <see cref="AnimationClock"/> which can generate the Animation.Clock.CurrentTime or 
		/// Animation.Clock.CurrentProgress value to be used by the animation to generate its output value.
		/// </param>
		/// <returns>
		/// The value this animation believes should be the current value for the property.
		/// </returns>
		public override object GetCurrentValue(
			object defaultOriginValue, 
			object defaultDestinationValue,
			AnimationClock animationClock)
		{
			ReadPreamble();

            if (animationClock == null)
            {
                throw new ArgumentNullException("animationClock");
            }

			if (animationClock.CurrentState == ClockState.Stopped
				|| !animationClock.CurrentProgress.HasValue)
			{
				return defaultOriginValue;
			}

			GradientBrush defaultOriginalBrush = defaultOriginValue as GradientBrush;
            GradientBrush defaultDestinationBrush = defaultDestinationValue as GradientBrush;

			if (gradientBrush == null)
			{
                AnimationBrush = defaultDestinationBrush;
			}
			if (AnimationBrush == null)
			{
				return defaultOriginValue;
			}

			double progress = animationClock.CurrentProgress.Value;

			IEasingFunction easingFunction = EasingFunction;
			if (easingFunction != null)
			{
				progress = easingFunction.Ease(progress);
			}

			if (!progress.IsBetween(0, 1))
			{
				return defaultOriginValue;
			}

			return GetCurrentValue(defaultOriginalBrush, defaultDestinationBrush, progress);
		}

		#endregion Overrides

		#region Properties

		/// <summary>
		/// Gets the current frozen copy of the <see cref="GradientBrush"/>.
		/// </summary>
		protected GradientBrush AnimationBrush
		{
			get
			{
				return gradientBrush;
			}
			private set
			{
				if (ReferenceEquals(gradientBrush, value))
				{
					return;
				}

				GradientBrush oldValue = gradientBrush;
				gradientBrush = value != null ? value.Clone() : null;
				if (gradientBrush != null)
				{
					gradientBrush.Freeze();
				}

                OnAnimationBrushChanged(oldValue);
			}
		}

		#endregion Properties

		#region Abstract

		/// <summary>
		/// Called when current freezed copy of the <see cref="GradientBrush"/> is changed.
		/// </summary>
		/// <param name="oldValue">
		/// The original <see cref="GradientBrush"/> value.
		/// </param>
        protected virtual void OnAnimationBrushChanged(GradientBrush oldValue)
		{ }

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
		protected abstract GradientBrush GetCurrentValue(GradientBrush defaultOriginalBrush, GradientBrush defaultDestinationBrush, double progress);

		#endregion Abstrct
	}
}

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using MotionRemix.Common;

namespace MotionRemix
{
    /// <summary>
    /// Provides HSB (hue, saturation, brightness) based transition animation.
    /// </summary>
    public sealed class ColorAnimationUsingHsb : ColorAnimationBase
    {
        #region Fields

        private bool reverseEffect;
        private float hueOffset;
        private float saturationOffset;
        private float brightnessOffset;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ColorAnimationUsingHsb"/>.
        /// </summary>
        public ColorAnimationUsingHsb()
        {
            reverseEffect = DefaultReverseEffect;
            hueOffset = DefaultHueOffset;
            saturationOffset = DefaultSaturationOffset;
            brightnessOffset = DefaultBrightnessOffset;
        }

        #endregion Constructors

        #region Dependency Properties

        #region HueOffset

        private const string HueOffsetPropertyName = "HueOffset";
        private const float DefaultHueOffset = 0f;

        /// <summary>
        /// Dependency property for HueOffset.
        /// </summary>
        public static readonly DependencyProperty HueOffsetProperty =
            DependencyProperty.Register(
                HueOffsetPropertyName,
                typeof(float),
                typeof(ColorAnimationUsingHsb),
                new FrameworkPropertyMetadata(
                    DefaultHueOffset, 
                    OnHueOffsetPropertyChanged));

        /// <summary>
        /// HueOffset property.
        /// </summary>
        public float HueOffset
        {
            get
            {
                return (float)GetValue(HueOffsetProperty);
            }
            set
            {
                SetValue(HueOffsetProperty, value);
            }
        }

        private static void OnHueOffsetPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            ColorAnimationUsingHsb target = element as ColorAnimationUsingHsb;
            if (target == null)
            {
                return;
            }

            float oldValue = (float)args.OldValue;
            float newValue = (float)args.NewValue;

            target.OnHueOffsetChanged(oldValue, newValue);
        }

        private void OnHueOffsetChanged(float oldValue, float newValue)
        {
            if (oldValue.IsCloseTo(newValue))
            {
                return;
            }

            // Normalize the value
            float value = newValue;
            if (value.Abs() > 360)
            {
                value %= 360;
            }

            hueOffset = value;
        }

        #endregion HueOffset

        #region SaturationOffset

        private const string SaturationOffsetPropertyName = "SaturationOffset";
        private const float DefaultSaturationOffset = 0f;

        /// <summary>
        /// Dependency property for SaturationOffset.
        /// </summary>
        public static readonly DependencyProperty SaturationOffsetProperty =
            DependencyProperty.Register(
                SaturationOffsetPropertyName,
                typeof(float),
                typeof(ColorAnimationUsingHsb),
                new FrameworkPropertyMetadata(
                    DefaultSaturationOffset, 
                    OnSaturationOffsetPropertyChanged));

        /// <summary>
        /// SaturationOffset property.
        /// </summary>
        public float SaturationOffset
        {
            get
            {
                return (float)GetValue(SaturationOffsetProperty);
            }
            set
            {
                SetValue(SaturationOffsetProperty, value);
            }
        }

        private static void OnSaturationOffsetPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            ColorAnimationUsingHsb target = element as ColorAnimationUsingHsb;
            if (target == null)
            {
                return;
            }

            float oldValue = (float)args.OldValue;
            float newValue = (float)args.NewValue;

            target.OnSaturationOffsetChanged(oldValue, newValue);
        }

        private void OnSaturationOffsetChanged(float oldValue, float newValue)
        {
            if (oldValue.IsCloseTo(newValue))
            {
                return;
            }

            // Normalize the value
            float value = newValue;
            if (value.Abs() > 100)
            {
                value %= 100;
            }

            saturationOffset = value;
        }

        #endregion SaturationOffset

        #region BrightnessOffset

        private const string BrightnessOffsetPropertyName = "BrightnessOffset";
        private const float DefaultBrightnessOffset = 0f;

        /// <summary>
        /// Dependency property for BrightnessOffset.
        /// </summary>
        public static readonly DependencyProperty BrightnessOffsetProperty =
            DependencyProperty.Register(
                BrightnessOffsetPropertyName,
                typeof(float),
                typeof(ColorAnimationUsingHsb),
                new FrameworkPropertyMetadata(
                    DefaultBrightnessOffset, 
                    OnBrightnessOffsetPropertyChanged));

        /// <summary>
        /// BrightnessOffset property.
        /// </summary>
        public float BrightnessOffset
        {
            get
            {
                return (float)GetValue(BrightnessOffsetProperty);
            }
            set
            {
                SetValue(BrightnessOffsetProperty, value);
            }
        }

        private static void OnBrightnessOffsetPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            ColorAnimationUsingHsb target = element as ColorAnimationUsingHsb;
            if (target == null)
            {
                return;
            }

            float oldValue = (float)args.OldValue;
            float newValue = (float)args.NewValue;

            target.OnBrightnessOffsetChanged(oldValue, newValue);
        }

        private void OnBrightnessOffsetChanged(float oldValue, float newValue)
        {
            if (oldValue.IsCloseTo(newValue))
            {
                return;
            }

            // Normalize the value
            float value = newValue;
            if (value.Abs() > 100)
            {
                value %= 100;
            }

            brightnessOffset = value;
        }

        #endregion BrightnessOffset

        #region ReverseEffect

        private const string ReverseEffectPropertyName = "ReverseEffect";
        private const bool DefaultReverseEffect = false;

        /// <summary>
        /// Dependency property for ReverseEffect.
        /// </summary>
        public static readonly DependencyProperty ReverseEffectProperty =
            DependencyProperty.Register(
                ReverseEffectPropertyName,
                typeof(bool),
                typeof(ColorAnimationUsingHsb),
                new FrameworkPropertyMetadata(
                    DefaultReverseEffect,
                    OnReverseEffectPropertyChanged));

        /// <summary>
        /// ReverseEffect property.
        /// </summary>
        public bool ReverseEffect
        {
            get
            {
                return (bool)GetValue(ReverseEffectProperty);
            }
            set
            {
                SetValue(ReverseEffectProperty, value);
            }
        }

        private static void OnReverseEffectPropertyChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            ColorAnimationUsingHsb target = element as ColorAnimationUsingHsb;
            if (target == null)
            {
                return;
            }

            bool oldValue = (bool)args.OldValue;
            bool newValue = (bool)args.NewValue;

            target.OnReverseEffectChanged(oldValue, newValue);
        }

        private void OnReverseEffectChanged(bool oldValue, bool newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            reverseEffect = newValue;
        }

        #endregion ReverseEffect

        #region EasingFunction

        private const string EasingFunctionPropertyName = "EasingFunction";

        /// <summary>
        /// Dependency property for EasingFunction
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                EasingFunctionPropertyName,
                typeof(IEasingFunction),
                typeof(ColorAnimationUsingHsb));

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
        /// Calculates the value this animation believes should be the current value for the property. 
        /// </summary>
        /// <param name="defaultOriginValue"> 
        /// This value is the suggested origin value provided to the animation
        /// to be used if the animation does not have its own concept of a
        /// start value. If this animation is the first in a composition chain
        /// this value will be the snapshot value if one is available or the 
        /// base property value if it is not; otherise this value will be the
        /// value returned by the previous animation in the chain with an 
        /// animationClock that is not Stopped. 
        /// </param>
        /// <param name="defaultDestinationValue"> 
        /// This value is the suggested destination value provided to the animation
        /// to be used if the animation does not have its own concept of an
        /// end value. This value will be the base value if the animation is
        /// in the first composition layer of animations on a property; 
        /// otherwise this value will be the output value from the previous
        /// composition layer of animations for the property. 
        /// </param> 
        /// <param name="animationClock">
        /// This is the animationClock which can generate the CurrentTime or 
        /// CurrentProgress value to be used by the animation to generate its
        /// output value.
        /// </param>
        /// <returns> 
        /// The value this animation believes should be the current value for the property.
        /// </returns> 
        protected override Color GetCurrentValueCore(
            Color defaultOriginValue,
            Color defaultDestinationValue,
            AnimationClock animationClock)
        {
            if (animationClock.CurrentState == ClockState.Stopped
                || !animationClock.CurrentProgress.HasValue)
            {
                return defaultOriginValue;
            }

            double progress = animationClock.CurrentProgress.Value;
            IEasingFunction easingFunction = EasingFunction;
            if (easingFunction != null)
            {
                progress = easingFunction.Ease(progress);
            }

            if (progress.IsLessThan(0)
                || progress.IsGreaterThan(1))
            {
                return defaultOriginValue;
            }

            // Need to double the pace if reverse effect is enabled
            float scaleFactor = (float)(reverseEffect ? progress * 2 : progress);

            float hueDelta = hueOffset;
            float saturationDelta = saturationOffset;
            float brightnessDelta = brightnessOffset;

            if (!reverseEffect 
                || scaleFactor.IsLessThanOrClose(1)) 
            {
                hueDelta *= scaleFactor;
                saturationDelta *= scaleFactor;
                brightnessDelta *= scaleFactor;
            }
            else
            {
                if (scaleFactor.IsGreaterThan(1))
                {
                    scaleFactor -= 1;

                    hueDelta -= hueDelta * scaleFactor;
                    saturationDelta -= saturationDelta * scaleFactor;
                    brightnessDelta -= brightnessDelta * scaleFactor;
                }
            }

            if (hueDelta.IsZero() 
                && saturationDelta.IsZero() 
                && brightnessDelta.IsZero())
            {
                // No changes
                return defaultOriginValue;
            }

            Color color = HsbColor.Offset(
                defaultDestinationValue, 
                hueDelta, 
                saturationDelta, 
                brightnessDelta);

            return color;
        }

        /// <summary>
        /// Clone the current instance.
        /// </summary>
        /// <returns>
        /// Cloned instance from the current.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ColorAnimationUsingHsb();
        }

        #endregion Overrides
    }
}

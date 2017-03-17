using System;

using MotionRemix.Common;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model for color animation using HSB.
    /// </summary>
    public class ColorUsingHsbViewModel : ViewModelBase
    {
        private float hueOffset;
        private float saturationOffset;
        private float brightnessOffset;
        private bool reverseEffect;

        /// <summary>
        /// Initializes a new instance of <see cref="ColorUsingHsbViewModel"/>.
        /// </summary>
        public ColorUsingHsbViewModel()
        {
            hueOffset = 0;
            saturationOffset = 0;
            brightnessOffset = 0;
            reverseEffect = true;
        }

        /// <summary>
        /// Gets or set the hue offset.
        /// </summary>
        public float HueOffset
        {
            get
            {
                return hueOffset;
            }
            set
            {
                if (hueOffset.IsCloseTo(value))
                {
                    return;
                }

                hueOffset = value;
                OnPropertyChanged("HueOffset");
            }
        }

        /// <summary>
        /// Gets or set the saturation offset.
        /// </summary>
        public float SaturationOffset
        {
            get
            {
                return saturationOffset;
            }
            set
            {
                if (saturationOffset.IsCloseTo(value))
                {
                    return;
                }

                saturationOffset = value;
                OnPropertyChanged("SaturationOffset");
            }
        }

        /// <summary>
        /// Gets or set the brightness offset.
        /// </summary>
        public float BrightnessOffset
        {
            get
            {
                return brightnessOffset;
            }
            set
            {
                if (brightnessOffset.IsCloseTo(value))
                {
                    return;
                }

                brightnessOffset = value;
                OnPropertyChanged("BrightnessOffset");
            }
        }

        /// <summary>
        /// Gets or set the reverse effect.
        /// </summary>
        public bool ReverseEffect
        {
            get
            {
                return reverseEffect;
            }
            set
            {
                if (reverseEffect == value)
                {
                    return;
                }

                reverseEffect = value;
                OnPropertyChanged("ReverseEffect");
            }
        }
    }
}

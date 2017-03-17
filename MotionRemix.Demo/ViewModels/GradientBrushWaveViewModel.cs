using System;

using MotionRemix.Common;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model for gradient brush wave animation.
    /// </summary>
    public class GradientBrushWaveViewModel : ViewModelBase
    {
        private int repeat;
        private double maximumOffset;

        /// <summary>
        /// Initializes a new instance of <see cref="GradientBrushWaveViewModel"/>
        /// </summary>
        public GradientBrushWaveViewModel()
        {
            repeat = 4;
            maximumOffset = 0.04;
        }

        /// <summary>
        /// Gets or sets the wave repeat.
        /// </summary>
        public int Repeat
        {
            get
            {
                return repeat;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Repeat");
                }
                if (repeat == value)
                {
                    return;
                }

                repeat = value;
                OnPropertyChanged("Repeat");
            }
        }

        /// <summary>
        /// Gets or sets the maximum offset.
        /// </summary>
        public double MaximumOffset
        {
            get
            {
                return maximumOffset;
            }
            set
            {
                if (!value.IsBetween(0, 0.5))
                {
                    throw new ArgumentOutOfRangeException("MaximumOffset");
                }
                if (maximumOffset.IsCloseTo(value))
                {
                    return;
                }

                maximumOffset = value;
                OnPropertyChanged("MaximumOffset");
            }
        }
    }
}

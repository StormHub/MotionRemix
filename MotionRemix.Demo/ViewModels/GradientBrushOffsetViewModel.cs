using System;
using System.ComponentModel;

using MotionRemix.Common;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model for gradient brush offset animation.
    /// </summary>
    public class GradientBrushOffsetViewModel : ViewModelBase
    {
        private double startOffset;
        private bool startAnimation;

        /// <summary>
        /// Initializes a new instance of <see cref="GradientBrushOffsetViewModel"/>.
        /// </summary>
        public GradientBrushOffsetViewModel()
        {
            startOffset = 0.2;
        }

        /// <summary>
        /// Inidcates whether the start the animation or not.
        /// </summary>
        public bool StartAnimation
        {
            get
            {
                return startAnimation;
            }
            set
            {
                if (startAnimation == value)
                {
                    return;
                }

                startAnimation = value;
                OnPropertyChanged("StartAnimation");
            }
        }

        /// <summary>
        /// Gets or sets the start offset.
        /// </summary>
        public double StartOffset
        {
            get
            {
                return startOffset;
            }
            set
            {
                if (!value.IsBetween(0, 1))
                {
                    throw new ArgumentOutOfRangeException("StartOffset");
                }
                
                startOffset = value;
                OnPropertyChanged("StartOffset");
            }
        }
    }
}

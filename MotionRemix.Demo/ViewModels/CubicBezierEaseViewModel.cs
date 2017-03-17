using System.Windows;
using System.Windows.Media.Animation;

using MotionRemix.Common;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model for cubic bezier easing.
    /// </summary>
    public class CubicBezierEaseViewModel : ViewModelBase
    {
        private Vector point0;
        private Vector point1;
        private Vector point2;
        private EasingMode easingMode;

        /// <summary>
        /// Initializes a new instance of <see cref="CubicBezierEaseViewModel"/>
        /// </summary>
        public CubicBezierEaseViewModel()
            : this(new Vector(10, 10), new Vector(195, 113), new Vector(221, 15), EasingMode.EaseIn)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="CubicBezierEaseViewModel"/>
        /// </summary>
        /// <param name="point0">The first point.</param>
        /// <param name="point1">The second point.</param>
        /// <param name="point2">The third point.</param>
        /// <param name="easingMode">The <see cref="EasingMode"/>.</param>
        public CubicBezierEaseViewModel(
            Vector point0, 
            Vector point1, 
            Vector point2, 
            EasingMode easingMode)
        {
            this.point0 = point0;
            this.point1 = point1;
            this.point2 = point2;
            this.easingMode = easingMode;
        }

        /// <summary>
        /// Gets or sets the point 0.
        /// </summary>
        public Vector Point0
        {
            get
            {
                return point0;
            }
            set
            {
                if (point0.IsCloseTo(value))
                {
                    return;
                }

                point0 = value;
                OnPropertyChanged("Point0");
            }
        }

        /// <summary>
        /// Gets or sets the point 1.
        /// </summary>
        public Vector Point1
        {
            get
            {
                return point1;
            }
            set
            {
                if (point1.IsCloseTo(value))
                {
                    return;
                }

                point1 = value;
                OnPropertyChanged("Point1");
            }
        }

        /// <summary>
        /// Gets or sets the point 2.
        /// </summary>
        public Vector Point2
        {
            get
            {
                return point2;
            }
            set
            {
                if (point2.IsCloseTo(value))
                {
                    return;
                }

                point2 = value;
                OnPropertyChanged("Point2");
            }
        }

        /// <summary>
        /// Gets or sets the easing mode.
        /// </summary>
        public EasingMode Mode
        {
            get
            {
                return easingMode;
            }
            set
            {
                if (easingMode == value)
                {
                    return;
                }

                easingMode = value;
                OnPropertyChanged("Mode");
            }
        }
    }
}

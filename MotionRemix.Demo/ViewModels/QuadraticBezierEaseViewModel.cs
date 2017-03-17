using System.Windows;
using System.Windows.Media.Animation;

using MotionRemix.Common;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model for quadratic bezier easing.
    /// </summary>
    public class QuadraticBezierEaseViewModel : CubicBezierEaseViewModel
    {
        private Vector point3;

        /// <summary>
        /// Initializes a new instance of <see cref="QuadraticBezierEaseViewModel"/>.
        /// </summary>
        public QuadraticBezierEaseViewModel()
            : base(new Vector(10, 20), new Vector(81, 109), new Vector(163, 105), EasingMode.EaseIn)
        {
            point3 = new Vector(154, 31);
        }

        /// <summary>
        /// Gets or sets the point 3.
        /// </summary>
        public Vector Point3
        {
            get
            {
                return point3;
            }
            set
            {
                if (point3.IsCloseTo(value))
                {
                    return;
                }

                point3 = value;
                OnPropertyChanged("Point3");
            }
        }
    }
}

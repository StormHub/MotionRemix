using System.Windows;

using MotionRemix.Converters;

namespace MotionRemix
{
    /// <summary>
    /// Converts boolean to visibility values.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BooleanToVisibilityConverter"/>
        /// </summary>
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}

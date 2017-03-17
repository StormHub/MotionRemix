using System.Windows.Media;

namespace MotionRemix.Common
{
    /// <summary>
    /// Provides color and brush related extension methods.
    /// </summary>
    public static class MediaExtensions
    {
        /// <summary>
        /// Gets the linear interpolation for the specified from <see cref="Color"/>
        /// to another <see cref="Color"/> by the progress between 0 and 1 inclusive.
        /// </summary>
        /// <param name="from">
        /// The from <see cref="Color"/>.
        /// </param>
        /// <param name="to">
        /// The to <see cref="Color"/>.
        /// </param>
        /// <param name="progress">
        /// The progress between 0 and 1 inclusive.
        /// </param>
        /// <returns>
        /// The linear interpolation for the specified from <see cref="Color"/> to 
        /// another <see cref="Color"/> by the progress between 0 and 1 inclusive.
        /// </returns>
        public static Color To(this Color from, Color to, double progress)
        {
            if (!progress.IsBetween(0, 1))
            {
                return from;
            }

            // Initial value
            if (progress.IsCloseTo(0))
            {
                return from;
            }

            // Target value
            if (progress.IsCloseTo(1))
            {
                return to;
            }

            // Scale the color in between
            Color color = from + (to - from) * (float)progress;

            return color;
        }
    }
}

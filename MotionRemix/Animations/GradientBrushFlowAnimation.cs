using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using MotionRemix.Common;

namespace MotionRemix
{
	/// <summary>
	/// Provides gradient flow step from left offset to right offset on offset 0 and
    /// right offset to left offset on offset 1 at the same time.
	/// </summary>
	public class GradientBrushFlowAnimation : GradientBrushAnimationBase
	{
		#region Fields

        private ReadOnlyCollection<GradientStop> stops;
        private GradientBrush brush;
        private double total;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="GradientBrushFlowAnimation"/>
		/// </summary>
        public GradientBrushFlowAnimation()
		{
            stops = null;
            brush = null;
            total = 0;
        }

		#endregion Constructors

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
                || stops.Count == 0
                || brush == null)
			{
                return defaultDestinationBrush;
			}

            double offset = progress * 2;
            bool reverse = offset.IsGreaterThan(1);
            if (reverse)
            {
                offset -= 1;
            }

            offset *= total;
            IList<GradientStop> stopList = !reverse 
                ? stops.ToList() 
                : stops.Reverse().ToList();

            foreach(GradientStop stop in stopList.Skip(1))
            {
                int i = stopList.IndexOf(stop);
                double distance = (stop.Offset - stopList[i - 1].Offset).Abs();
                if (offset < distance)
                {
                    offset /= distance;
                    int j = (stopList.Count - 1) - i;

                    brush.GradientStops[0].Color = stopList[i - 1].Color.To(stopList[i].Color, offset);
                    brush.GradientStops[1].Color = stopList[j + 1].Color.To(stopList[j].Color, offset);

                    break;
                }
                else
                {
                    offset -= distance;
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
            stops = null;
            brush = null;
            total = 0;

            if (AnimationBrush == null 
                || AnimationBrush.GradientStops == null 
                || AnimationBrush.GradientStops.Count == 0)
            {
                return;
            }

            List<GradientStop> list = new List<GradientStop>();
            foreach (GradientStop stop in 
                AnimationBrush.GradientStops.OrderBy(value => value.Offset))
            {
                if (!list.Exists(
                        value => value.Offset.IsCloseTo(stop.Offset)))
                {
                    list.Add(stop);
                }
            }

            if (list.Count < 2)
            {
                return;
            }

            for (int i = 1; i < list.Count; i++)
            {
                total += list[i].Offset - list[i - 1].Offset;
            }
            stops = new ReadOnlyCollection<GradientStop>(list);

            brush = AnimationBrush.Clone();
            brush.GradientStops.Clear();

            brush.GradientStops.Add(new GradientStop(stops.First().Color, 0));
            brush.GradientStops.Add(new GradientStop(stops.Last().Color, 1));
        }

		/// <summary>
        /// Create new instance of <see cref="GradientBrushFlowAnimation"/>.
		/// </summary>
		/// <returns>
        /// The new instance of <see cref="GradientBrushFlowAnimation"/>.
		/// </returns>
		protected override Freezable CreateInstanceCore()
		{
            return new GradientBrushFlowAnimation();
		}

		#endregion Overrides
	}
}

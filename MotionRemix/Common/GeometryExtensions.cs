using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MotionRemix.Common
{
	/// <summary>
	/// Provide path geomerty related extensions.
	/// </summary>
	public static class GeometryExtensions
	{
		private const double FullCircleInDegrees = 360;

		/// <summary>
		/// Increases the angle by the delta and ensure the final result is in
		/// -360 to 360 degrees.
		/// </summary>
		/// <param name="angle">
		/// The angle in degrees to increase.
		/// </param>
		/// <param name="delta">
		/// The delta angle in degree to increase by.
		/// </param>
		/// <returns>
		/// The angle increased by delta and ensure the final result is in
		/// -360 to 360 degrees.
		/// </returns>
		public static double Angle(this double angle, double delta)
		{
			double normalizeAngle = angle.Normalize();
			double value = normalizeAngle + delta;
			return value.Normalize();
		}

		/// <summary>
		/// Converts the percent from 0 to 100 into proportional angle from 0 to 360.
		/// </summary>
		/// <param name="percent">
		/// The percent to convert.
		/// </param>
		/// <returns>
		/// The converted angle from 0 to 360 proportional to 0 to 100 percent.
		/// </returns>
		public static double Angle(this double percent)
		{
			if (percent.IsLessThan(0)
				|| percent.IsGreaterThan(100))
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Percent '{0}' must be between 0 to 100", percent));
			}

			return (FullCircleInDegrees / 100) * percent;
		}

		/// <summary>
		/// Creates a circle path for the specified location, angle in degrees, circle radius and inner radius.
		/// </summary>
		/// <param name="location">
		/// The start location.
		/// </param>
		/// <param name="angle">
		/// The angle in degrees.
		/// </param>
		/// <param name="radius">
		/// The radius.
		/// </param>
		/// <param name="innerRadius">
		/// Inner radius.
		/// </param>
		/// <returns>
		/// The circle path for the specified location, angle in degrees, circle radius and inner radius.
		/// </returns>
		public static PathGeometry CreatePath(this Point location, double angle, double radius, double innerRadius)
		{
			if (radius.IsLessThan(0))
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Radius '{0}' must be greater than zero.", radius));
			}
			if (innerRadius.IsLessThan(0))
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Inner radius '{0}' must be greater than zero.", innerRadius));
			}

			bool isLargeArc = angle > FullCircleInDegrees / 2;

			PathGeometry pathGeometry = new PathGeometry();
			PathSegmentCollection segments = new PathSegmentCollection();

			Point arcPoint = ConvertRadianToCartesian(angle, radius);
			Point innerArcPoint = ConvertRadianToCartesian(angle, innerRadius);

			segments.Add(new LineSegment(
				new Point(location.X, location.Y - radius), false));
			segments.Add(new ArcSegment(
				new Point(location.X + arcPoint.X, location.Y + arcPoint.Y),
				new Size(radius, radius),
				0,
				isLargeArc,
				SweepDirection.Clockwise,
				false));

			segments.Add(new LineSegment(
				new Point(location.X + innerArcPoint.X, location.Y + innerArcPoint.Y), false));
			segments.Add(new ArcSegment(
				new Point(location.X, location.Y - innerRadius),
				new Size(innerRadius, innerRadius),
				0,
				isLargeArc,
				SweepDirection.Counterclockwise,
				false));

			PathFigure figure = new PathFigure(location, segments, true);
			pathGeometry.Figures = new PathFigureCollection
                {
                    figure
                };

			return pathGeometry;
		}

		/// <summary>
		/// Creates a circle path spilits into the given number of sigments.
		/// </summary>
		/// <param name="point">
		/// The start location.
		/// </param>
		/// <param name="segments">
		/// Number of sigments.
		/// </param>
		/// <param name="margin">
		/// Sigment distance between each other in degrees.
		/// </param>
		/// <param name="radius">
		/// The radius.
		/// </param>
		/// <param name="innerRadius">
		/// The inner radius.
		/// </param>
		/// <returns>
		/// The combined path geomerty of the circle spilits into the number of segments.
		/// </returns>
		public static PathGeometry Create(this Point point, int segments, double margin, double radius, double innerRadius)
		{
			if (segments <= 0)
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Segments '{0}' must be greater than zero.", segments));
			}

			if (margin.IsLessThan(0)
				|| margin.IsGreaterThan(360))
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Margin '{0}' must be greater than zero and less than 360.", margin));
			}

			if (radius.IsLessThan(0))
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Radius '{0}' must be greater than zero.", radius));
			}

			if (innerRadius.IsLessThan(0))
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Inner radius '{0}' must be greater than zero.", innerRadius));
			}

			double angleSegment = (360d / segments) - margin;
			PathGeometry pathGeometry = new PathGeometry();

			double angle = margin / 2;
			for (int i = 0; i < segments; i++)
			{
				PathGeometry geometry = point.CreatePath(angleSegment, radius, innerRadius);
				geometry.Transform = new RotateTransform(angle, point.X, point.Y);
				PathGeometry segmentGeometry = geometry.GetFlattenedPathGeometry();
				pathGeometry.AddGeometry(segmentGeometry);

				angle += (margin + angleSegment);
			}

			return pathGeometry;
		}

		/// <summary>
		/// Gets the radians angle for the start and end points.
		/// </summary>
		/// <param name="start">
		/// The start point.
		/// </param>
		/// <param name="end">
		/// The end point.
		/// </param>
		/// <returns>
		/// The radians angle for the start and end points.
		/// </returns>
		public static double DistanceTo(this Point start, Point end)
		{
			Vector vector = end - start;
			return vector.Length();
		}

		/// <summary>
		/// Gets the vector point for the specified angle in degrees and radius.
		/// </summary>
		/// <param name="angle">
		/// The angle in degrees.
		/// </param>
		/// <param name="radius">
		/// The radius.
		/// </param>
		/// <returns>
		/// The vector point for the specified angle in degrees and radius.
		/// </returns>
		public static Point ConvertRadianToCartesian(this double angle, double radius)
		{
			if (radius.IsLessThan(0))
			{
				throw new ArgumentOutOfRangeException(
					string.Format("Radius '{0}' must be greater than zero.", radius));
			}

			var angleRadius = (Math.PI / (FullCircleInDegrees / 2)) * (angle - FullCircleInDegrees / 4);
			var x = radius * Math.Cos(angleRadius);
			var y = radius * Math.Sin(angleRadius);
			return new Point(x, y);
		}

		/// <summary>
		/// Normalizes the specified angle in degrees to angles between 0 to 360;
		/// </summary>
		/// <param name="angle">
		/// The angle to normalize.
		/// </param>
		/// <returns>
		/// Normalized angle in degrees from 0 to 360 for the specified <paramref name="angle"/>
		/// </returns>
		public static double Normalize(this double angle)
		{
			double remainder = angle % FullCircleInDegrees;

			if (remainder.IsGreaterThanOrCloseTo(FullCircleInDegrees))
			{
				return (remainder - FullCircleInDegrees);
			}

			if (remainder.IsLessThan(0))
			{
				remainder += FullCircleInDegrees;
			}

			return remainder;
		}

		/// <summary>
		/// Gets the radians angle for the start and end points.
		/// </summary>
		/// <param name="start">
		/// The start point.
		/// </param>
		/// <param name="end">
		/// The end point.
		/// </param>
		/// <returns>
		/// The radians angle for the start and end points.
		/// </returns>
		public static double Angle(this Point start, Point end)
		{
			return ((Vector) start).Angle((Vector) end);
		}

		/// <summary>
		/// Gets the radians angle for the start and end points.
		/// </summary>
		/// <param name="start">
		/// The start point.
		/// </param>
		/// <param name="end">
		/// The end point.
		/// </param>
		/// <returns>
		/// The radians angle for the start and end points.
		/// </returns>
		public static double Angle(this Vector start, Vector end)
		{
			Vector vector = end - start;
			return vector.X.Atan2(vector.Y);
		}

		/// <summary>
		/// Converts radians to angle.
		/// </summary>
		/// <param name="radians">
		/// The radians to convert.
		/// </param>
		/// <returns>
		/// The converted angle from radians.
		/// </returns>
		public static double ToAngle(this double radians)
		{
			if (!radians.IsFiniteValue())
			{
				throw new ArgumentOutOfRangeException("radians");
			}

			return (radians * 180.0) / Math.PI;
		}

		/// <summary>
		/// Converts angle to radians.
		/// </summary>
		/// <param name="angle">
		/// The angle to convert.
		/// </param>
		/// <returns>
		/// The converted radians from angle.
		/// </returns>
		public static double ToRadians(this double angle)
		{
			if (!angle.IsFiniteValue())
			{
				throw new ArgumentOutOfRangeException("angle");
			}

			return (angle * Math.PI) / 180;
		}

		/// <summary>
		/// Indicates whether the specified <see cref="Rect"/> has value of zero
		/// for both height and width.
		/// </summary>
		/// <param name="rect">
		/// The <see cref="Rect"/> to check for.
		/// </param>
		/// <returns>
		/// True if the rect has value of zero on both height and width. Otherwise false.
		/// </returns>
		public static bool IsZero(this Rect rect)
		{
			return rect.Height.IsEqualOrCloseToZero()
				&& rect.Width.IsEqualOrCloseToZero();
		}

		/// <summary>
		/// Indicates whether the specified <see cref="Vector"/> has value of zero
		/// for both x and y.
		/// </summary>
		/// <param name="vector">
		/// The vector to check for.
		/// </param>
		/// <returns>
		/// True if the vector has value of zero on both x and y. Otherwise false.
		/// </returns>
		public static bool IsZero(this Vector vector)
		{
			return vector.X.IsEqualOrCloseToZero() 
				&& vector.X.IsEqualOrCloseToZero();
		}

		/// <summary>
		/// Rotates the given <see cref="Point"/> for the specified degrees.
		/// </summary>
		/// <param name="point">
		/// The <see cref="Point"/> to rotate.
		/// </param>
		/// <param name="degrees">
		/// Degrees to roate.
		/// </param>
		/// <returns>
		/// Rotated the given <paramref name="point"/> for the specified <paramref name="degrees"/>.
		/// </returns>
		public static Point Rotate(this Point point, double degrees)
		{
			Vector vector = ((Vector)point).Rotate(degrees);
			return (Point)vector;
		}

		/// <summary>
		/// Rotates the given <see cref="Vector"/> for the specified degrees.
		/// </summary>
		/// <param name="vector">
		/// The <see cref="Vector"/> to rotate.
		/// </param>
		/// <param name="degrees">
		/// Degrees to roate.
		/// </param>
		/// <returns>
		/// Rotated the given <paramref name="vector"/> for the specified <paramref name="degrees"/>.
		/// </returns>
		public static Vector Rotate(this Vector vector, double degrees)
		{
			double radians = (degrees * Math.PI) / 180.0;
			double num2 = Math.Sin(radians);
			double num3 = Math.Cos(radians);
			double x = (vector.X * num3) - (vector.Y * num2);
			return new Vector(x, (vector.X * num2) + (vector.Y * num3));
		}

		/// <summary>
		/// Calculates the length of the specified <see cref="Vector"/>.
		/// </summary>
		/// <param name="vector">
		/// The <see cref="Vector"/> to calculate.
		/// </param>
		/// <returns>
		/// The length of the specified <see cref="Vector"/>.
		/// </returns>
		public static double Length(this Vector vector)
		{
			return ((vector.X * vector.X) + (vector.Y * vector.Y)).Sqrt();
		}

        /// <summary>
        /// Combines a list of <see cref="PathSegment"/> to <see cref="Geometry"/>.
        /// </summary>
        /// <param name="segments">
        /// The list of <see cref="PathSegment"/> to combine.
        /// </param>
        /// <returns>
        /// Combined <see cref="Geometry"/> for a list of <see cref="PathSegment"/>.
        /// If the segements is empty or null, empty geometry will be returned.
        /// </returns>
		public static PathGeometry Combine(this IReadOnlyCollection<PathSegment> segments)
        {
            PathFigure pathFigure = new PathFigure();

	        if (segments != null 
				&& segments.Count > 0)
	        {
		        foreach (PathSegment segment in segments)
		        {
			        pathFigure.Segments.Add(segment);
		        }
	        }

	        PathGeometry pathGeometry = new PathGeometry
	        {
		        Figures = new PathFigureCollection
		        {
			        pathFigure
		        }
	        };

            return pathGeometry;
        }
	}
}

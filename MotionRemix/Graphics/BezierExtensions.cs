
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using MotionRemix.Common;

namespace MotionRemix.Graphics
{
	/// <summary>
	/// Provides bezier based extension methods.
	/// </summary>
	public static class BezierExtensions
	{
        /// <summary>
        /// Creates combined <see cref="Geometry"/> from a list of <see cref="Bezier"/> instances.
        /// </summary>
        /// <param name="bezierList">
        /// The list of <see cref="Bezier"/> instances.
        /// </param>
        /// <returns>
        /// the combined <see cref="Geometry"/> from a list of <see cref="Bezier"/> instances.
        /// </returns>
        public static Geometry CreateGeometry(this IReadOnlyCollection<Bezier> bezierList)
        {
            if (bezierList == null)
            {
                return Geometry.Empty;
            }

            PathFigure pathFigure = new PathFigure();
            foreach (Bezier bezier in bezierList)
            {
                if (pathFigure.Segments.Count == 0)
                {
                    pathFigure.StartPoint = (Point)bezier.P0;
                }

                BezierSegment segment = new BezierSegment(
                    (Point)bezier.P1, 
                    (Point)bezier.P2, 
                    (Point)bezier.P3, 
                    true);
                pathFigure.Segments.Add(segment);
            }

            PathGeometry pathGeometry = new PathGeometry
            {
                Figures = new PathFigureCollection
				{
					pathFigure
				}
            };

            return pathGeometry.GetFlattenedPathGeometry();
        }

        /// <summary>
        /// Filters a list of points based on distance for the specified tolerance.
        /// </summary>
        /// <param name="points">
        /// A list of <see cref="Point"/> to filter.
        /// </param>
        /// <param name="tolerance">
        /// The minimum tolerance of the distance between points.
        /// </param>
        /// <returns>
        /// A list of points based on distance for the specified tolerance.
        /// </returns>
        public static List<Point> Filter(this IEnumerable<Point> points, int tolerance)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException("tolerance");
            }

            List<Point> list = new List<Point>();
            foreach (Point point in points)
            {
                if (list.Count == 0 
                    || point
                        .DistanceTo(list.Last())
                        .IsGreaterThan(tolerance))
                {
                    list.Add(point);
                }
            }

            return list;
        }

        /// <summary>
        /// Calculates the left/start tangent <see cref="Vector"/> from a list of <see cref="Point"/>.
        /// </summary>
        /// <param name="points">
        /// A list of <see cref="Point"/> to calculate.
        /// </param>
        /// <returns>
        /// The left/start tangent <see cref="Vector"/> from a list of <see cref="Point"/>.
        /// </returns>
        public static Vector LeftTangent(this ReadOnlyCollection<Point> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Count < 2)
            {
                throw new ArgumentException("There must be at least two points to calculate left tangent value.", "points");
            }

            Vector tangent = points[1] - points.First();
            tangent.Normalize();

            return tangent;
        }

        /// <summary>
        /// Calculates the right/end tangent <see cref="Vector"/> from a list of <see cref="Point"/>.
        /// </summary>
        /// <param name="points">
        /// A list of <see cref="Point"/> to calculate.
        /// </param>
        /// <returns>
        /// The right/end tangent <see cref="Vector"/> from a list of <see cref="Point"/>.
        /// </returns>
        public static Vector RightTangent(this ReadOnlyCollection<Point> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Count < 2)
            {
                throw new ArgumentException("There must be at least two points to calculate right tangent value.", "points");
            }

            Vector tangent = points[points.Count - 2] - points.Last();
            tangent.Normalize();

            return tangent;
        }

        /// <summary>
        /// Calculates the center tangent <see cref="Vector"/> from a list of <see cref="Point"/>
        /// at the specified index.
        /// </summary>
        /// <param name="points">
        /// A list of <see cref="Point"/> to calculate.
        /// </param>
        /// <param name="index">
        /// The center point index.
        /// </param>
        /// <returns>
        /// The center tangent <see cref="Vector"/> from a list of <see cref="Point"/> at the 
        /// specified index.
        /// </returns>
        public static Vector CenterTangent(this ReadOnlyCollection<Point> points, int index)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Count < 3)
            {
                throw new ArgumentException("There must be at least two points to calculate center tangent value.", "points");
            }
            if (index < 1 
                || index >= points.Count - 2)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Vector left = points[index - 1] - points[index];
            Vector right = points[index] - points[index + 1];

            Vector tangent = 
                new Vector(
                    (left.X + right.X) / 2.0, 
                    (left.Y + left.Y) / 2.0);
            tangent.Normalize();

            return tangent;
        }

		/// <summary>
		/// Calculates a list of chord length values from a list of points for the specified start and end index.
		/// </summary>
		/// <param name="points">
		/// The list of <see cref="Point"/> to calculate distance values.
		/// </param>
		/// <returns>
		/// The list of distance values from a list of points for the specified start and end index.
		/// </returns>
        public static ReadOnlyCollection<double> Parameterize(this IEnumerable<Point> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            List<double> values = new List<double>();
            Point? start = null;
            foreach (Point point in points)
            {
                values.Add(
                    start.HasValue
                    ? values.Last() + start.Value.DistanceTo(point)
                    : 0d);
                start = point;
            }

            for (int i = 1; i < values.Count; i++)
            {
                values[i] /= values.Last();
            }

            return values.AsReadOnly();
        }

        /// <summary>
        /// Gets the <see cref="Point"/> on the bezier curve defined by a list of <see cref="Point"/> 
        /// at the specified t.
        /// </summary>
		/// <param name="points">
        /// A list of <see cref="Point"/> defines the bezier curve.
        /// </param>
        /// <param name="t">
        /// The t value to evaluate.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/> on the bezier curve defined by a list of <see cref="Point"/> 
        /// at the specified t.
        /// </returns>
        public static Point At(this IEnumerable<Point> points, double t)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            List<Point> list = points.ToList();
            if (list.Count == 0)
            {
                throw new ArgumentException("There must be at least one point for the bezier curve.", "points");
            }
            int degree = list.Count - 1;

            // Local copy of control points
            Point[] values = new Point[list.Count + 1];
            for (int i = 0; i <= degree; i++)
            {
                values[i] = list[i];
            }

            // Triangle computation
            for (int i = 1; i <= degree; i++)
            {
                for (int j = 0; j <= degree - i; j++)
                {
                    values[j].X = (1.0 - t) * values[j].X + t * values[j + 1].X;
                    values[j].Y = (1.0 - t) * values[j].Y + t * values[j + 1].Y;
                }
            }

            return values[0];
        }

		/// <summary>
		/// Use Newton-Raphson iteration to find better root for the <see cref="Bezier"/>.
		/// </summary>
		/// <param name="bezier">
		/// The <see cref="Bezier"/> instance.
		/// </param>
		/// <param name="point">
		/// The <see cref="Point"/> approximate to.
		/// </param>
		/// <param name="u">
		/// The current value.
		/// </param>
		/// <returns>
		/// The improved value.
		/// </returns>
		public static double NewtonRaphsonRootFind(this Bezier bezier, Point point, double u)
		{
			Point[] q1 = new Point[3]; // Q'
			Point[] q2 = new Point[2]; // Q''

			Point[] points = bezier.Points;

			// Compute Q(u)
			Point qu = points.At(u);

			// Generate control vertices for Q'
			for (int i = 0; i <= 2; i++)
			{
				q1[i].X = (points[i + 1].X - points[i].X) * 3.0;
				q1[i].Y = (points[i + 1].Y - points[i].Y) * 3.0;
			}

			// Generate control vertices for Q''
			for (int i = 0; i <= 1; i++)
			{
				q2[i].X = (q1[i + 1].X - q1[i].X) * 2.0;
				q2[i].Y = (q1[i + 1].Y - q1[i].Y) * 2.0;
			}

			// Compute Q'(u) and Q''(u)
			Point q1_u = q1.Take(2).At(u);
			Point q2_u = q2.Take(1).At(u);

			// Compute f(u)/f'(u)
			double numerator = 
				(qu.X - point.X) * (q1_u.X) 
					+ (qu.Y - point.Y) * (q1_u.Y);

			double denominator = 
				(q1_u.X) * (q1_u.X) + (q1_u.Y) * (q1_u.Y) 
					+ (qu.X - point.X) * (q2_u.X) 
					+ (qu.Y - point.Y) * (q2_u.Y);

			return 
				denominator.IsCloseTo(0)
				? u
				: u - (numerator / denominator); // Improved u = u - f(u)/f'(u)
		}

        /// <summary>
        /// Calculates the cubic bezier B0 multiplier value for the specified u.
        /// </summary>
        /// <param name="u">
        /// The value to calculate.
        /// </param>
        /// <returns>
        /// The cubic bezier B0 multiplier value for the specified u.
        /// </returns>
        public static double B0(this double u)
        {
            double tmp = 1.0 - u;
            return (tmp * tmp * tmp);
        }

        /// <summary>
        /// Calculates the cubic bezier B1 multiplier value for the specified u.
        /// </summary>
        /// <param name="u">
        /// The value to calculate.
        /// </param>
        /// <returns>
        /// The cubic bezier B1 multiplier value for the specified u.
        /// </returns>
        public static double B1(this double u)
        {
            double tmp = 1.0 - u;
            return (3 * u * (tmp * tmp));
        }

        /// <summary>
        /// Calculates the cubic bezier B2 multiplier value for the specified u.
        /// </summary>
        /// <param name="u">
        /// The value to calculate.
        /// </param>
        /// <returns>
        /// The cubic bezier B2 multiplier value for the specified u.
        /// </returns>
        public static double B2(this double u)
        {
            double tmp = 1.0 - u;
            return (3 * u * u * tmp);
        }

        /// <summary>
        /// Calculates the cubic bezier B3 multiplier value for the specified u.
        /// </summary>
        /// <param name="u">
        /// The value to calculate.
        /// </param>
        /// <returns>
        /// The cubic bezier B3 multiplier value for the specified u.
        /// </returns>
        public static double B3(this double u)
        {
            return (u * u * u);
        }
	}
}
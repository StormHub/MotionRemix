
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using MotionRemix.Common;

namespace MotionRemix.Graphics
{
	/// <summary>
	/// Cubic Bezier curve in 2D consisting of 4 control points.
	/// </summary>
	public struct Bezier : IEquatable<Bezier>
	{
		#region Fields

		private readonly Vector p0;
		private readonly Vector p1;
		private readonly Vector p2;
        private readonly Vector p3;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="Bezier"/>.
		/// </summary>
		/// <param name="p0">
		/// Left end <see cref="Point"/> point of the bezier.
		/// </param>
		/// <param name="p1">
		/// Left <see cref="Point"/> control point of the bezier.
		/// </param>
		/// <param name="p2">
		/// Right <see cref="Point"/> control point of the bezier.
		/// </param>
		/// <param name="p3">
		/// Right end <see cref="Point"/> point of the bezier.
		/// </param>
        public Bezier(Point p0, Point p1, Point p2, Point p3)
            : this((Vector)p0, (Vector)p1, (Vector)p2, (Vector)p3)
        {  }

        /// <summary>
        /// Initializes a new instance of <see cref="Bezier"/>.
        /// </summary>
        /// <param name="p0">
        /// Left end <see cref="Vector"/> point of the bezier.
        /// </param>
        /// <param name="p1">
        /// Left <see cref="Vector"/> control point of the bezier.
        /// </param>
        /// <param name="p2">
        /// Right <see cref="Vector"/> control point of the bezier.
        /// </param>
        /// <param name="p3">
        /// Right end <see cref="Vector"/> point of the bezier.
        /// </param>
        public Bezier(Vector p0, Vector p1, Vector p2, Vector p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

		#endregion Constructors

		#region Properties

		/// <summary>
		/// The start <see cref="Point"/> of the bezier.
		/// </summary>
        public Vector P0
		{
			get
			{
				return p0;
			}
		}

		/// <summary>
		/// The first control <see cref="Point"/> of the bezier.
		/// </summary>
        public Vector P1
		{
			get
			{
				return p1;
			}
		}

		/// <summary>
		/// The second control <see cref="Point"/> of the bezier.
		/// </summary>
        public Vector P2
		{
			get
			{
				return p2;
			}
		}

		/// <summary>
		/// The end <see cref="Point"/> of the bezier.
		/// </summary>
        public Vector P3
		{
			get
			{
				return p3;
			}
		}

		/// <summary>
		/// Gets all the bezier points.
		/// </summary>
		public Point[] Points
		{
			get
			{
				return new Point[]
				{
					(Point) p0,
					(Point) p1,
					(Point) p2,
					(Point) p3
				};
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Creates a new instance of <see cref="Bezier"/> for the specified <see cref="BezierBuilder"/>.
		/// </summary>
		/// <param name="bezierBuilder">
		/// The <see cref="BezierBuilder"/> instance.
		/// </param>
		/// <returns>
		/// The new instance of <see cref="Bezier"/> for the specified <see cref="BezierBuilder"/>.
		/// </returns>
		internal static Bezier Create(BezierBuilder bezierBuilder)
		{
			if (bezierBuilder == null)
			{
				throw new ArgumentNullException("bezierBuilder");
			}

			ReadOnlyCollection<Point> points = bezierBuilder.Points;
			ReadOnlyCollection<double> parameterizedLength = bezierBuilder.ParameterizedLength;

            Debug.Assert(points.Count == parameterizedLength.Count);

            Vector leftTangent = bezierBuilder.LeftTangent;
            Vector rightTangent = bezierBuilder.RightTangent;

			// Compute the A's  
			List<VectorPair> matrixA = new List<VectorPair>();
			for (int i = 0; i < points.Count; i++)
			{
				VectorPair pair = new VectorPair(leftTangent, rightTangent, parameterizedLength[i]);
				matrixA.Add(pair);
			}

			// Matrix C
			double[,] mc = new double[2, 2];
			// Matrix X 
			double[] mx = new double[2];

			// Create the C and X matrices
			mc[0, 0] = 0.0;
			mc[0, 1] = 0.0;
			mc[1, 0] = 0.0;
			mc[1, 1] = 0.0;
			mx[0] = 0.0;
			mx[1] = 0.0;

			for (int i = 0; i < points.Count; i++)
			{
				mc[0, 0] += matrixA[i].DotFirst();
				mc[0, 1] += matrixA[i].Dot();
				mc[1, 0] = mc[0, 1];
				mc[1, 1] += matrixA[i].DotSecond();

				Vector vector =
					(Vector)points[i]
					- (
						((Vector)points.First() * parameterizedLength[i].B0())
						+ (
							((Vector)points.First() * parameterizedLength[i].B1())
							+ (
								((Vector)points.Last() * parameterizedLength[i].B2())
									+ ((Vector)points.Last() * parameterizedLength[i].B3())
							  )
						  )
					  );

				mx[0] += matrixA[i].DotFirst(vector);
				mx[1] += matrixA[i].DotSecond(vector);
			}

			// Compute the determinants of C and X matrices  

			double det_C0_C1 = mc[0, 0] * mc[1, 1] - mc[1, 0] * mc[0, 1];
			double det_C0_X = mc[0, 0] * mx[1] - mc[1, 0] * mx[0];
			double det_X_C1 = mx[0] * mc[1, 1] - mx[1] * mc[0, 1];

			// Finally, derive alpha values, left and right
			double alpha_l = det_C0_C1.IsCloseTo(0) ? 0.0 : det_X_C1 / det_C0_C1;
			double alpha_r = det_C0_C1.IsCloseTo(0) ? 0.0 : det_C0_X / det_C0_C1;

			//
			// If alpha negative, use the Wu/Barsky heuristic (see text) 
			// (if alpha is 0, you get coincident control points that lead to
			// divide by zero in any subsequent NewtonRaphsonRootFind() call.
			//

			Point p0 = points.First();
			Point p3 = points.Last();

			double segmentLength = points.First().DistanceTo(points.Last());
			double epsilon = 1.0e-6 * segmentLength;
			if (alpha_l < epsilon || alpha_r < epsilon)
			{
				//
				// Fall back on standard (probably inaccurate) formula, 
				// and subdivide further if needed.
				//
				double distance = segmentLength / 3.0;
				return new Bezier(
					p0,
					(leftTangent * distance) + p0,
					(rightTangent * distance) + p3,
					p3);
			}

			//
			//  First and last control points of the Bezier curve are
			//  positioned exactly at the first and last data points
			//  Control points 1 and 2 are positioned an alpha distance out
			//  on the tangent vectors, left and right, respectively
			//
			return new Bezier(
				p0,
				(leftTangent * alpha_l) + p0,
				(rightTangent * alpha_r) + p3,
				p3);
		}

		/// <summary>
		/// Calculates the maximum deviation of points to this instance.
		/// </summary>
		/// <param name="bezierBuilder">
		/// The <see cref="BezierBuilder"/> instance.
		/// </param>
		/// <param name="index">
		/// The index of the maximum error point.
		/// </param>
		/// <returns>
		/// The maximum deviation of points to this instance.
		/// </returns>
		internal double CalculateMaximumDistance(BezierBuilder bezierBuilder, out int index)
		{
			if (bezierBuilder == null)
			{
				throw new ArgumentNullException("bezierBuilder");
			}

			ReadOnlyCollection<Point> points = bezierBuilder.Points;
			ReadOnlyCollection<double> parameterizedLength = bezierBuilder.ParameterizedLength;

			Debug.Assert(points.Count == parameterizedLength.Count);

			double maximumDistanceError = 0.0;
			index = points.Count / 2;

			Point[] bezierPoints =
			{
				(Point) p0,
				(Point) p1,
				(Point) p2,
				(Point) p3
			};

			for (int i = 1; i < points.Count; i++)
			{
				Point point = bezierPoints.At(parameterizedLength[i]);
				Vector vector = point - points[i];
				double distance = vector.LengthSquared;

				if (distance >= maximumDistanceError)
				{
					maximumDistanceError = distance;
					index = i;
				}
			}

			return maximumDistanceError;
		}

		/// <summary>
		/// Reparameterization by Newton-Raphson iteration to find better root for lengh values.
		/// </summary>
		/// <param name="bezierBuilder">
		/// The <see cref="BezierBuilder"/> instance.
		/// </param>
		/// <returns>
		/// The reparameterization by Newton-Raphson iteration to find better root for lengh values.
		/// </returns>
		internal ReadOnlyCollection<double> Reparameterize(BezierBuilder bezierBuilder)
		{
			if (bezierBuilder == null)
			{
				throw new ArgumentNullException("bezierBuilder");
			}

			ReadOnlyCollection<Point> points = bezierBuilder.Points;
			ReadOnlyCollection<double> parameterizedLength = bezierBuilder.ParameterizedLength;

			Debug.Assert(points.Count == parameterizedLength.Count);

			List<double> values = new List<double>();
			for (int index = 0; index < points.Count; index++)
			{
				double value = this.NewtonRaphsonRootFind(points[index], parameterizedLength[index]);
				values.Add(value);
			}

			return values.AsReadOnly();
		}

		#endregion Methods

		#region Overrrides

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			return p0.GetHashCode()
				^ p1.GetHashCode()
				^ p2.GetHashCode()
				^ p3.GetHashCode();
		}

		/// <summary>
		/// Gets the string based representation of the object.
		/// </summary>
		/// <returns>
		/// The string based representation of the object.
		/// </returns>
		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append("Bezier: (<");

			buffer.Append(p0.X.ToString("#.###", CultureInfo.InvariantCulture));
			buffer.Append(", ");
			buffer.Append(p0.Y.ToString("#.###", CultureInfo.InvariantCulture));

			buffer.Append("> <");
			buffer.Append(p1.X.ToString("#.###", CultureInfo.InvariantCulture));
			buffer.Append(", ");
			buffer.Append(p1.Y.ToString("#.###", CultureInfo.InvariantCulture));

			buffer.Append("> <");
			buffer.Append(p2.X.ToString("#.###", CultureInfo.InvariantCulture));
			buffer.Append(", ");
			buffer.Append(p2.Y.ToString("#.###", CultureInfo.InvariantCulture));
			buffer.Append("> <");

			buffer.Append(p3.X.ToString("#.###", CultureInfo.InvariantCulture));
			buffer.Append(", ");
			buffer.Append(p3.Y.ToString("#.###", CultureInfo.InvariantCulture));
			buffer.Append(">)");

			return buffer.ToString();
		}

		#endregion Overrides

		#region Equality Implementation

		/// <summary>
		/// Compares two <see cref="Bezier"/> instances for equality.
		/// </summary>
		/// <param name="left">
		/// The first <see cref="Bezier"/> to compare.
		/// </param>
		/// <param name="right">
		/// The second <see cref="Bezier"/> to compare.
		/// </param>
		/// <returns>
		/// True if the two instances are identical. Otherwise false.
		/// </returns>
		public static bool operator ==(Bezier left, Bezier right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two <see cref="Bezier"/> instances for inequality.
		/// </summary>
		/// <param name="left">
		/// The first <see cref="Bezier"/> to compare.
		/// </param>
		/// <param name="right">
		/// The second <see cref="Bezier"/> to compare.
		/// </param>
		/// <returns>
		/// True if the two instances are not identical. Otherwise false.
		/// </returns>
		public static bool operator !=(Bezier left, Bezier right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Compares to another <see cref="Bezier"/> for equality.
		/// </summary>
		/// <param name="other">
		/// The <see cref="Bezier"/> to compare to.
		/// </param>
		/// <returns>
		/// True if the two instances are identical. Otherwise false.
		/// </returns>
		public bool Equals(Bezier other)
		{
            return Vector.Equals(p0, other.p0)
                && Vector.Equals(p1, other.p1)
                && Vector.Equals(p2, other.p2)
                && Vector.Equals(p3, other.p3);
		}

		/// <summary>
		/// Compares to another object for equality.
		/// </summary>
		/// <param name="obj">
		/// The object instance to compare to.
		/// </param>
		/// <returns>
		/// True if the two instances are identical. Otherwise false.
		/// </returns>
		public override bool Equals(object obj)
		{
			return obj is Bezier && Equals((Bezier)obj);
		}

		#endregion Equality Implementation
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using MotionRemix.Common;

namespace MotionRemix.Graphics
{
	/// <summary>
	/// Provides bezier based approximation functionalities.
	/// </summary>
    public sealed class BezierBuilder
    {
        #region Constants

        // Minimum 2 pixels between points
        private const int MinimumDistance = 2; 

        private const double MinimumError = 4;
        private const double MaximumError = 20;
        private const double DefaultError = 8;

		private const double MaximumIterationCount = 4;

        #endregion Constants

        #region Fields

        private readonly ReadOnlyCollection<Point> points;
        private readonly Vector leftTangent;
        private readonly Vector rightTangent;
        private readonly double tolerance;

        private ReadOnlyCollection<double> parameterizedLength;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="BezierBuilder"/> for the specified list of
        /// <see cref="Point"/> and error tolerance.
        /// </summary>
        /// <param name="points">
        /// A list of <see cref="Point"/> to build from.
        /// </param>
        /// <param name="leftTangent">
        /// The left tangent <see cref="Vector"/>.
        /// </param>
        /// <param name="rightTangent">
        /// The right tangent <see cref="Vector"/>.
        /// </param>
        /// <param name="tolerance">
        /// The error tolerance value between 4 to 20 inclusive.
        /// </param>
        private BezierBuilder(
            ReadOnlyCollection<Point> points,
            Vector leftTangent,
            Vector rightTangent,
            double tolerance = DefaultError)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (points.Count < 2)
            {
                throw new ArgumentException("There must be at least two distinct points in the list.", "points");
            }

            if (!tolerance.IsBetween(MinimumError, MaximumError))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "The error tolerance must be between {0} and {1} inclusive.", 
                        MinimumError, 
                        MaximumError), 
                    "tolerance");
            }

            this.points = points;
            this.leftTangent = leftTangent;
            this.rightTangent = rightTangent;
            this.tolerance = tolerance;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
		/// Get the list of <see cref="Point"/> to fit.
		/// </summary>
		internal ReadOnlyCollection<Point> Points
		{
			get
			{
				return points;
			}
		}

		/// <summary>
		/// Gets the parameterized length values.
		/// </summary>
		internal ReadOnlyCollection<double> ParameterizedLength
		{
			get
			{
				return parameterizedLength;
			}
		}

        /// <summary>
        /// Gets the left tangent <see cref="Vector"/>.
        /// </summary>
        internal Vector LeftTangent
        {
            get
            {
                return leftTangent;
            }
        }

        /// <summary>
        /// Gets the right tangent <see cref="Vector"/>.
        /// </summary>
        internal Vector RightTangent
        {
            get
            {
                return rightTangent;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Builds a list of <see cref="Bezier"/> from a list of <see cref="Point"/> for
        /// the specified error tolerance.
        /// </summary>
        /// <param name="points">
        /// A list of <see cref="Point"/> to build from.
        /// </param>
        /// <param name="tolerance">
        /// The error tolerance value between 4 to 20 inclusive.
        /// </param>
        /// <returns>
        /// A list of <see cref="Bezier"/> from a list of <see cref="Point"/> for
        /// the specified error tolerance.
        /// </returns>
        public static ReadOnlyCollection<Bezier> Build(
            IEnumerable<Point> points,
            double tolerance = DefaultError)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (!tolerance.IsBetween(MinimumError, MaximumError))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "The error tolerance must be between {0} and {1} inclusive.",
                        MinimumError,
                        MaximumError),
                    "tolerance");
            }

            List<Point> list = points.Filter(MinimumDistance);
            if (list.Count < 2)
            {
                throw new ArgumentException("There must be at least two distinct points in the list.", "points");
            }

            List<Bezier> bezierList = new List<Bezier>();

            ReadOnlyCollection<Point> bezierPoints = list.AsReadOnly();
            Vector leftTangent = bezierPoints.LeftTangent();
            Vector rightTangent = bezierPoints.RightTangent();

            BezierBuilder bezierBuilder = new BezierBuilder(
                bezierPoints, 
                leftTangent, 
                rightTangent, 
                tolerance);
            bezierBuilder.Build(bezierList);

            return bezierList.AsReadOnly();
        }

        private void Build(List<Bezier> bezierList)
        {
            // Use heuristic if region only has two points in it
            if (points.Count == 2)
            {
                Point p0 = points.First();
                Point p3 = points.Last();

                double distance = p0.DistanceTo(p3) / 3.0;
                Point p1 = p0 + leftTangent * distance;
                Point p2 = p3 + rightTangent * distance;

                bezierList.Add(new Bezier(p0, p1, p2, p3));
                return;
            }

            // Parameterize points, and attempt to fit curve
            parameterizedLength = points.Parameterize();
            Bezier bezier = Bezier.Create(this);

            // Find max deviation of points to fitted curve
            int index;
            double maximumDistanceError = bezier.CalculateMaximumDistance(this, out index);
	        if (maximumDistanceError < tolerance)
	        {
                bezierList.Add(bezier);
		        return;
	        }

			//  If error not too large, try some reparameterization and iteration
	        double iterationError = tolerance * tolerance;
	        if (maximumDistanceError < iterationError)
	        {
				for (int i = 0; i < MaximumIterationCount; i++)
		        {
                    parameterizedLength = bezier.Reparameterize(this);
                    bezier = Bezier.Create(this);
                    maximumDistanceError = bezier.CalculateMaximumDistance(this, out index);
			        if (maximumDistanceError < tolerance)
			        {
                        bezierList.Add(bezier);
				        return;
			        }
		        }
	        }

            // Fitting failed -- split at max error point and fit recursively
            Vector centerTangent = points.CenterTangent(index);

            BezierBuilder builder = new BezierBuilder(
                points
                    .Take(index + 1)                    
                    .ToList()
                    .AsReadOnly(), 
                leftTangent,
                centerTangent,
                tolerance);
            builder.Build(bezierList);

            centerTangent.Negate();
            builder = new BezierBuilder(
                points
                    .Skip(index + 1)
                    .ToList()
                    .AsReadOnly(),
                centerTangent,
                rightTangent,
                tolerance);
            builder.Build(bezierList);
        }

        #endregion Methods
    }
}

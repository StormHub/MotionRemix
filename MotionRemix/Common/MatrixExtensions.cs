using System;
using System.Windows;

namespace MotionRemix.Common
{
	/// <summary>
	/// Utility to provide matrix related double calculations.
	/// </summary>
	public static class MatrixExtensions
	{
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
		/// Indicates whether the specified <see cref="Rect"/> has value of zero
		/// for either height or width.
		/// </summary>
		/// <param name="rect">
		/// The <see cref="Rect"/> to check for.
		/// </param>
		/// <returns>
		/// True if the rect has value of zero on either height or width. Otherwise false.
		/// </returns>
		public static bool IsZero(this Rect rect)
		{
			return rect.Height.IsEqualOrCloseToZero()
				|| rect.Width.IsEqualOrCloseToZero();
		}
	}
}

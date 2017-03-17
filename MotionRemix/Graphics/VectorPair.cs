
using System.Windows;

namespace MotionRemix.Graphics
{
	/// <summary>
	/// Represents the <see cref="Vector"/> pair for A's.
	/// </summary>
	public struct VectorPair
	{
		private readonly Vector first;
		private readonly Vector second;

		/// <summary>
		/// Initializes a new instance of <see cref="VectorPair"/>.
		/// </summary>
		/// <param name="first">
		/// The first <see cref="Vector"/> value.
		/// </param>
		/// <param name="second">
		/// The second <see cref="Vector"/> values.
		/// </param>
		/// <param name="length">
		/// The length to calculate bezier B1 and B2.
		/// </param>
		public VectorPair(Vector first, Vector second, double length)
		{
			this.first = first * length.B1();
			this.second = second * length.B2();
		}

		/// <summary>
		/// Gets the cross product of the first <see cref="Vector"/>.
		/// </summary>
		/// <returns>
		/// The cross product of the first <see cref="Vector"/>.
		/// </returns>
		public double DotFirst()
		{
			return Vector.CrossProduct(first, first);
		}

		/// <summary>
		/// Gets the cross product of the first <see cref="Vector"/> and the 
		/// specified <see cref="Vector"/>.
		/// </summary>
		/// <param name="vector">
		/// The <see cref="Vector"/> to calculate.
		/// </param>
		/// <returns>
		/// The cross product of the first <see cref="Vector"/> and the 
		/// specified <see cref="Vector"/>.
		/// </returns>
		public double DotFirst(Vector vector)
		{
			return Vector.CrossProduct(first, vector);
		}

		/// <summary>
		/// Gets the cross product of the second <see cref="Vector"/>.
		/// </summary>
		/// <returns>
		/// The cross product of the second <see cref="Vector"/>.
		/// </returns>
		public double DotSecond()
		{
			return Vector.CrossProduct(second, second);
		}

		/// <summary>
		/// Gets the cross product of the second <see cref="Vector"/> and the
		/// specified <see cref="Vector"/>.
		/// </summary>
		/// <param name="vector">
		/// The <see cref="Vector"/> to calculate.
		/// </param>
		/// <returns>
		/// The cross product of the second <see cref="Vector"/>  and the
		/// specified <see cref="Vector"/>.
		/// </returns>
		public double DotSecond(Vector vector)
		{
			return Vector.CrossProduct(second, vector);
		}

		/// <summary>
		/// Gets the cross product of the <see cref="Vector"/> pair.
		/// </summary>
		/// <returns>
		/// The cross product of the two <see cref="Vector"/> pair.
		/// </returns>
		public double Dot()
		{
			return Vector.CrossProduct(first, second);
		}
	}
}

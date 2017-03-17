
using System;
using System.Collections.Generic;
using System.Linq;

namespace MotionRemix.Common
{
	/// <summary>
	/// Provides random generation related extension methods.
	/// </summary>
	public static class RandomExtension
	{
        /// <summary>
        /// Generates the specified number of unique random numbers between 0 and 1.
        /// </summary>
        /// <param name="random">
        /// The <see cref="Random"/> instance.
        /// </param>
        /// <param name="count">
        /// The number count.
        /// </param>
        /// <returns>
        /// The specified number of unique random numbers between 0 and 1.
        /// </returns>
        public static List<double> NextValues(this Random random, int count)
        {
            if (random == null)
            {
                throw new ArgumentNullException("random");
            }
            if (count < 2)
            {
                throw new ArgumentNullException("count", "The count must be at least 2.");
            }

            List<double> list = new List<double>(count);

            for (int i = 0; i < count - 1; i++)
            {
                double value = random.NextDouble();
                if (list.Count == 0)
                {
                    list.Add(value);
                }
                else
                {
                    list.Add(value + value * (1 - list.Last()));
                }
            }

            return list;
        }

		/// <summary>
		/// Shuffle a list of item using Fisher–Yates. 
		/// That is a random permutation of a finite set.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the list.
		/// </typeparam>
		/// <param name="random">
		/// The <see cref="Random"/> instance.
		/// </param>
		/// <param name="list">
		/// The Shuffle
		/// </param>
		public static void Shuffle<T>(this Random random, IList<T> list)
		{
			if (random == null)
			{
				throw new ArgumentNullException("random");
			}
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}

			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = (int) (random.NextDouble() * (n + 1));
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		/// <summary>
		/// Generates normally distributed numbers.
		/// </summary>
		/// <param name="random">
		/// The <see cref="Random"/> instance.
		/// </param>
		/// <param name = "mu">
		/// Mean of the distribution
		/// </param>
		/// <param name = "sigma">
		/// Standard deviation.
		/// </param>
		/// <returns>
		/// The normally distributed number.
		/// </returns>
		public static double Normal(this Random random, double mu = 0, double sigma = 1)
		{
			if (random == null)
			{
				throw new ArgumentNullException("random");
			}

			double u1 = random.NextDouble();
			double u2 = random.NextDouble();

			//random normal(0,1)
			double stdNormal = (-2.0 * u1.Log()).Sqrt() * (2.0 * Math.PI * u2).Sin();

			//random normal(mean, stdDev ^ 2)
			double normal = mu + sigma * stdNormal;

			return normal;
		}
	}
}

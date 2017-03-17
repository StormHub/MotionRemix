using System.Windows;
using System.Windows.Media.Animation;

using MotionRemix.Common;
using MotionRemix.Graphics;

namespace MotionRemix
{
	/// <summary>
	/// Provides quadratic bezier easing along the projected length from the
	/// bezier vector from P0 to P3.
	/// </summary>
	public class QuadraticBezierEase : EasingFunctionBase
	{
		#region Fields

		private Vector point0;
		private Vector point1;
		private Vector point2;
		private Vector point3;
		private Vector direction;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="QuadraticBezierEase"/>.
		/// </summary>
		public QuadraticBezierEase()
		{
			point0 = new Vector(0, 0);
			point1 = new Vector(0, 0);
			point2 = new Vector(0, 0);
			point3 = new Vector(0, 0);
			direction = new Vector(0, 0);
		}

		#endregion Constructors

		#region Dependency Properties

		#region Point0

		private const string Point0PropertyName = "Point0";

		/// <summary>
		/// Dependency property for Point0.
		/// </summary>
		public static readonly DependencyProperty Point0Property =
			DependencyProperty.Register(
				Point0PropertyName,
				typeof(Point),
				typeof(QuadraticBezierEase),
				new FrameworkPropertyMetadata(
					new Point(0, 0), 
					OnPoint0PropertyChanged));

		/// <summary>
		/// Point0 property
		/// </summary>
		public Point Point0
		{
			get
			{
				return (Point)GetValue(Point0Property);
			}
			set
			{
				SetValue(Point0Property, value);
			}
		}

		private static void OnPoint0PropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			QuadraticBezierEase target = element as QuadraticBezierEase;
			if (target == null)
			{
				return;
			}

			Point oldValue = (Point)args.OldValue;
			Point newValue = (Point)args.NewValue;

			target.OnPoint0PropertyChanged(oldValue, newValue);
		}

		#endregion Point0

		#region Point1

		private const string Point1PropertyName = "Point1";

		/// <summary>
		/// Dependency property for Point1.
		/// </summary>
		public static readonly DependencyProperty Point1Property =
			DependencyProperty.Register(
				Point1PropertyName,
				typeof(Point),
				typeof(QuadraticBezierEase),
				new FrameworkPropertyMetadata(
					new Point(0, 0),
					OnPoint1PropertyChanged));

		/// <summary>
		/// Point1 property
		/// </summary>
		public Point Point1
		{
			get
			{
				return (Point)GetValue(Point1Property);
			}
			set
			{
				SetValue(Point1Property, value);
			}
		}

		private static void OnPoint1PropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			QuadraticBezierEase target = element as QuadraticBezierEase;
			if (target == null)
			{
				return;
			}

			Point oldValue = (Point)args.OldValue;
			Point newValue = (Point)args.NewValue;

			target.OnPoint1PropertyChanged(oldValue, newValue);
		}

		#endregion Point1

		#region Point2

		private const string Point2PropertyName = "Point2";

		/// <summary>
		/// Dependency property for Point2.
		/// </summary>
		public static readonly DependencyProperty Point2Property =
			DependencyProperty.Register(
				Point2PropertyName,
				typeof(Point),
				typeof(QuadraticBezierEase),
				new FrameworkPropertyMetadata(
					new Point(0, 0),
					OnPoint2PropertyChanged));

		/// <summary>
		/// Point2 property
		/// </summary>
		public Point Point2
		{
			get
			{
				return (Point)GetValue(Point2Property);
			}
			set
			{
				SetValue(Point2Property, value);
			}
		}

		private static void OnPoint2PropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			QuadraticBezierEase target = element as QuadraticBezierEase;
			if (target == null)
			{
				return;
			}

			Point oldValue = (Point)args.OldValue;
			Point newValue = (Point)args.NewValue;

			target.OnPoint2PropertyChanged(oldValue, newValue);
		}

		#endregion Point2

		#region Point3

		private const string Point3PropertyName = "Point3";

		/// <summary>
		/// Dependency property for Point3.
		/// </summary>
		public static readonly DependencyProperty Point3Property =
			DependencyProperty.Register(
				Point3PropertyName,
				typeof(Point),
				typeof(QuadraticBezierEase),
				new FrameworkPropertyMetadata(
					new Point(0, 0),
					OnPoint3PropertyChanged));

		/// <summary>
		/// Point3 property
		/// </summary>
		public Point Point3
		{
			get
			{
				return (Point)GetValue(Point3Property);
			}
			set
			{
				SetValue(Point3Property, value);
			}
		}

		private static void OnPoint3PropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			QuadraticBezierEase target = element as QuadraticBezierEase;
			if (target == null)
			{
				return;
			}

			Point oldValue = (Point)args.OldValue;
			Point newValue = (Point)args.NewValue;

			target.OnPoint3PropertyChanged(oldValue, newValue);
		}

		#endregion Point2

		#endregion Dependency Properties

		#region Overrides

		/// <summary>
		/// Transforms normalized time to control the pace of an animation.
		/// </summary> 
		/// <param name="normalizedTime">
		/// Normalized time (progress) of the animation
		/// </param>
		/// <returns>
		/// The transformed progress.
		/// </returns> 
		protected override double EaseInCore(double normalizedTime)
		{
			Vector bt =
				  normalizedTime.B0() * point0 
				+ normalizedTime.B1() * point1 
				+ normalizedTime.B2() * point2 
				+ normalizedTime.B3() * point3;

			Vector vector = bt - point0;
			double angle = direction.Angle(vector);
			vector.Normalize();
			double projection = vector.Length * angle.Cos();

			if (projection.IsLessThan(0))
			{
				return projection.Abs();
			}

			if (projection.IsGreaterThan(1))
			{
				return 2 - projection;
			}

			return projection;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Freezable"/> class.
		/// </summary>
		/// <returns>
		/// The new instance.
		/// </returns>
		protected override Freezable CreateInstanceCore()
		{
			return new QuadraticBezierEase();
		}

		#endregion Overrides

		#region Property Change Handlers

		private void OnPoint0PropertyChanged(Point oldValue, Point newValue)
		{
			if (newValue.IsCloseTo(oldValue))
			{
				return;
			}

			point0 = (Vector)newValue;
			direction = point3 - point0;
		}

		private void OnPoint1PropertyChanged(Point oldValue, Point newValue)
		{
			if (newValue.IsCloseTo(oldValue))
			{
				return;
			}

			point1 = (Vector)newValue;
		}

		private void OnPoint2PropertyChanged(Point oldValue, Point newValue)
		{
			if (newValue.IsCloseTo(oldValue))
			{
				return;
			}

			point2 = (Vector)newValue;
		}

		private void OnPoint3PropertyChanged(Point oldValue, Point newValue)
		{
			if (newValue.IsCloseTo(oldValue))
			{
				return;
			}

			point3 = (Vector)newValue;
			direction = point3 - point0;
		}

		#endregion Property Change Handlers
	}
}

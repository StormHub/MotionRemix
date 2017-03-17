using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using MotionRemix.Common;

namespace MotionRemix
{
    /// <summary>
	/// Provides  logarithmic spiral shape defined by 
	///		r = a * E ^ (b * Radians) 
    /// in polar coordinates, 
    /// where and a and b being arbitrary positive real constants.
    /// </summary>
    public class Spiral : Shape
    {
        #region Fields

        private const int MaximumRadiusValue = 20;
        private const double MinimumAngleIncrement = Math.PI / 180;
        private const double MinimumRadiusIncrement = 0.5;

        private Rect boundRect;
	    private Point center;

	    private double startAngle;
	    private double stopAngle;
		private double coefficiency;
	    private double velocity;

        private Geometry geometry;
        private Pen strokePen;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Spiral"/>.
        /// </summary>
        public Spiral()
        {
            boundRect = Rect.Empty;
	        startAngle = DefaultStartAngle;
	        stopAngle = DefaultStopAngle;
			coefficiency = DefaultCoefficiency;
        }

        #endregion Constructors

		#region Dependency Properties

		#region StartAngle Property

		private const string StartAnglePropertyName = "StartAngle";
	    private const double DefaultStartAngle = 0d;

        /// <summary>
        /// Dependency property for StartAngle.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(
                StartAnglePropertyName, 
                typeof(double), 
                typeof(Spiral),
                new FrameworkPropertyMetadata(
					DefaultStartAngle,
					FrameworkPropertyMetadataOptions.AffectsArrange 
						| FrameworkPropertyMetadataOptions.AffectsMeasure 
						| FrameworkPropertyMetadataOptions.AffectsRender,
                    StartAnglePropertyChanged),
                IsValidStartAngle);

        /// <summary>
        /// Gets or sets the start angle in degrees of the <see cref="Spiral"/>.
        /// </summary>
        /// <returns>
        /// The start angle in degrees of the <see cref="Spiral"/>.
        /// </returns>
        public double StartAngle
        {
            get
            {
				return (double)GetValue(StartAngleProperty);
            }
            set
            {
				SetValue(StartAngleProperty, value);
            }
        }

        private static void StartAnglePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            Spiral target = element as Spiral;
            if (target == null)
            {
                return;
            }

            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;

            target.OnStartAnglePropertyChanged(oldValue, newValue);
        }

        private static bool IsValidStartAngle(object value)
        {
            double target = (double)value;
            return target.IsFiniteValue();
        }

        #endregion StartAngle Property

        #region StopAngle Property

        private const string StopAnglePropertyName = "StopAngle";
	    private const double DefaultStopAngle = double.NaN;

        /// <summary>
        /// Dependency property for StopAngle.
        /// </summary>
        public static readonly DependencyProperty StopAngleProperty =
            DependencyProperty.Register(
                StopAnglePropertyName,
                typeof(double),
                typeof(Spiral),
                new FrameworkPropertyMetadata(
					DefaultStopAngle,
					FrameworkPropertyMetadataOptions.AffectsArrange
						| FrameworkPropertyMetadataOptions.AffectsMeasure
						| FrameworkPropertyMetadataOptions.AffectsRender,
					StopAnglePropertyChanged));

        /// <summary>
        /// Gets or sets the stop angle in degrees of the <see cref="Spiral"/>.
        /// </summary>
        /// <returns>
        /// The stop angle in degrees of the <see cref="Spiral"/>.
        /// </returns>
        public double StopAngle
        {
            get
            {
				return (double)GetValue(StopAngleProperty);
            }
            set
            {
				SetValue(StopAngleProperty, value);
            }
        }

        private static void StopAnglePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            Spiral target = element as Spiral;
            if (target == null)
            {
                return;
            }

            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;

            target.OnStopAnglePropertyChanged(oldValue, newValue);
        }

        #endregion StopAngle Property

		#region Coefficiency Property

		private const string CoefficiencyPropertyName = "Coefficiency";
		private const double DefaultCoefficiency = 1d;

		/// <summary>
		/// Dependency property for Coefficiency.
		/// </summary>
		public static readonly DependencyProperty CoefficiencyProperty =
			DependencyProperty.Register(
				CoefficiencyPropertyName,
				typeof(double),
				typeof(Spiral),
				new FrameworkPropertyMetadata(
					DefaultCoefficiency,
					FrameworkPropertyMetadataOptions.AffectsArrange
						| FrameworkPropertyMetadataOptions.AffectsMeasure
						| FrameworkPropertyMetadataOptions.AffectsRender,
					CoefficiencyPropertyChanged),
                IsValidCoefficiency);

		/// <summary>
		/// Gets or sets the coefficient constant of the <see cref="Spiral"/>.
		/// </summary>
		/// <returns>
		/// The coefficient constant of the <see cref="Spiral"/>.
		/// </returns>
		public double Coefficiency
		{
			get
			{
				return (double)GetValue(CoefficiencyProperty);
			}
			set
			{
				SetValue(CoefficiencyProperty, value);
			}
		}

		private static void CoefficiencyPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
		{
			Spiral target = element as Spiral;
			if (target == null)
			{
				return;
			}

			double oldValue = (double)args.OldValue;
			double newValue = (double)args.NewValue;

			target.OnCoefficiencyPropertyChanged(oldValue, newValue);
		}

        private static bool IsValidCoefficiency(object value)
        {
            double target = (double)value;
            return target.IsFiniteValue() 
                && target.IsGreaterThan(0);
        }

		#endregion Coefficiency Property

		#region Velocity Property

		private const string VelocityPropertyName = "Velocity";
	    private const double DefaultVelocity = 02d;

        /// <summary>
        /// Dependency property for Velocity.
        /// </summary>
        public static readonly DependencyProperty VelocityProperty =
            DependencyProperty.Register(
                VelocityPropertyName,
                typeof(double),
                typeof(Spiral),
                new FrameworkPropertyMetadata(
					DefaultVelocity,
					FrameworkPropertyMetadataOptions.AffectsArrange
						| FrameworkPropertyMetadataOptions.AffectsMeasure
						| FrameworkPropertyMetadataOptions.AffectsRender,
					VelocityPropertyChanged),
                IsValidVelocity);

        /// <summary>
        /// Gets or sets the velocity constant of the <see cref="Spiral"/>.
        /// </summary>
        /// <returns>
        /// The velocity constant of the <see cref="Spiral"/>.
        /// </returns>
        public double Velocity
        {
            get
            {
				return (double)GetValue(VelocityProperty);
            }
            set
            {
				SetValue(VelocityProperty, value);
            }
        }

        private static void VelocityPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            Spiral target = element as Spiral;
            if (target == null)
            {
                return;
            }

            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;

            target.OnVelocityPropertyChanged(oldValue, newValue);
        }

        private static bool IsValidVelocity(object value)
        {
            double target = (double)value;
            return target.IsFiniteValue()
                && target.IsGreaterThan(0);
        }

        #endregion Velocity Property

		#endregion Dependency Properties

		#region Overrides

		/// <summary>
        /// Gets a value that represents the <see cref="Geometry"/> of <see cref="Spiral"/>.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                return (boundRect.IsEmpty || geometry == null)
                    ? Geometry.Empty
                    : geometry;
            }
        }

        /// <summary>
        /// Measures all child sizes for the specified <see cref="Size"/>.
        /// </summary>
        /// <param name="constraint">
        /// The available size.
        /// </param>
        /// <returns>
        /// The measured size for all child items.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (Stretch != Stretch.UniformToFill)
            {
                return GetNaturalSize();
            }

            double width = constraint.Width;
            double height = constraint.Height;

            if (double.IsInfinity(width)
                && double.IsInfinity(height))
            {
                return GetNaturalSize();
            }

            double radius = double.IsInfinity(width) || double.IsInfinity(height) 
                    ? Math.Min(width, height) 
					: Math.Max(width, height);

            return new Size(radius, radius);
        }

        /// <summary>
        /// Arranges the size for the specified available <see cref="Size"/>.
        /// </summary>
        /// <param name="finalSize">
        /// The final <see cref="Size"/> of the  panel.
        /// </param>
        /// <returns>
        /// The arranged size for all child items.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double strokeThickness = StrokeThickness.Abs();
            double margin = strokeThickness / 2.0;

            boundRect = new Rect(
                margin, 
                margin, 
                Math.Max(0.0, finalSize.Width - strokeThickness), 
                Math.Max(0.0, finalSize.Height - strokeThickness));

			center = new Point(boundRect.Width / 2, boundRect.Height / 2);

            geometry = null;
            return finalSize;
        }

        /// <summary>
        /// Provides a means to change the default appearance of a <see cref="Spiral"/> element.
        /// </summary>
        /// <param name="drawingContext">
        /// A System.Windows.Media.DrawingContext object that is drawn during the rendering
        /// pass of <see cref="Spiral"/>.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (boundRect.IsEmpty)
            {
                return;
            }
           
            Pen pen = GetPen();
            if (pen != null)
            {
                GenerateGeometry();

                if (geometry != null 
                    && !geometry.IsEmpty())
                {
                    drawingContext.DrawGeometry(Fill, pen, geometry);
                }
            }
        }

        #endregion Overrides

        #region Property Change Handlers

        private void OnStartAnglePropertyChanged(double oldValue, double newValue)
        {
            if (newValue.IsCloseTo(oldValue))
            {
                return;
            }

			Refresh();
		}

        private void OnStopAnglePropertyChanged(double oldValue, double newValue)
        {
            if (newValue.IsCloseTo(oldValue))
            {
                return;
            }

			Refresh();
		}

		private void OnCoefficiencyPropertyChanged(double oldValue, double newValue)
		{
			if (newValue.IsCloseTo(oldValue))
			{
				return;
			}
			Refresh();
		}

		private void OnVelocityPropertyChanged(double oldValue, double newValue)
		{
			if (newValue.IsCloseTo(oldValue))
			{
				return;
			}

			Refresh();
		}

	    private void Refresh()
	    {
		    geometry = null;

            startAngle = StartAngle;
			coefficiency = Coefficiency;
		    velocity = Velocity;

            double value = StopAngle;
            if (value.IsFiniteValue() 
                && value > startAngle)
		    {
                stopAngle = value;
		    }
            else
            {
                stopAngle = DefaultStopAngle;
            }

            startAngle = startAngle.ToRadians();
            if (!stopAngle.IsNaNValue())
            {
                stopAngle = stopAngle.ToRadians();
            }
			
			InvalidateMeasure();
	    }

		#endregion Property Change Handlers

		#region Geometry

		private void GenerateGeometry()
		{
            if (geometry != null 
                || boundRect.IsEmpty 
				|| boundRect.IsZero())
            {
                return;
            }

			if (coefficiency.IsCloseTo(0))
			{
				geometry = Geometry.Empty;
				return;
			}

			List<Point> points = new List<Point>();

			// If the start angle is specified, we dont need
			// the center point to be the first point.
            if (startAngle.IsCloseTo(DefaultStartAngle))
			{
				points.Add(center);
			}

			double currentAngle = startAngle;
			double radius = coefficiency * (velocity * currentAngle).Exp();
			Point point;
			while (!StopGeometryPath(radius, currentAngle, out point))
            {
                points.Add(point);

                //
                // When the radius of spiral shape is small, 
                // angle increment is smoother.
                //
                // Or when the velocity is zero where the result 
                // is a circle, angle increment is the only option.
                //
                if (radius.IsLessThanOrCloseTo(MaximumRadiusValue) 
                    || velocity.IsEqualOrCloseToZero())
                {
                    currentAngle += MinimumAngleIncrement;
					radius = coefficiency * (velocity * currentAngle).Exp();
                }
                else
                {
                    // 
                    // When the radius of spiral shape is big,
                    // radius increment is smoother.
                    //
                    radius += MinimumRadiusIncrement;
					currentAngle = (radius / coefficiency).Log() / velocity;
                }
            }

			if (points.Count < 2)
			{
				geometry = Geometry.Empty;
				return;
			}

            GeometryGroup geometryGroup = new GeometryGroup();
			Point start = points[0];
			for(int i = 1; i < points.Count; i++)
			{
                //
                // Optimisation, if the distance of two points
                // is less than or close to 3 pixels, keep going
                // to the next.
                // 
                double distance = start.DistanceTo(points[i]);
                if (distance.IsGreaterThan(3))
                {
                    LineGeometry line = new LineGeometry(start, points[i]);
                    geometryGroup.Children.Add(line);
                    start = points[i];
                }
			}
            geometry = geometryGroup.GetFlattenedPathGeometry();
			geometry.Freeze();
        }

        private bool StopGeometryPath(double radius, double angle, out Point point)
        {
			double x = radius * angle.Cos();
			double y = radius * angle.Sin();

			point = new Point(center.X + x, center.Y + y);

			// Keep the geometry inside the rendering boundary
			if (!boundRect.Contains(point))
	        {
				return true;
			}
			
			// Stop angle check
	        if (!stopAngle.IsNaNValue())
	        {
		        return angle.IsGreaterThan(stopAngle);
	        }

            return false;
        }

		#endregion Geometry

		#region Rendering Resources

		private Pen GetPen()
        {
            double strokeThickness = StrokeThickness;
            if (Stroke == null
                || strokeThickness.IsNaNValue()
                || strokeThickness.IsEqualOrCloseToZero())
            {
                return null;
            }

            if (strokePen != null)
            {
                return strokePen;
            }

            double thickness = strokeThickness.Abs();

            strokePen = new Pen
            {
	            Thickness = thickness,
	            Brush = Stroke,
	            StartLineCap = StrokeStartLineCap,
	            EndLineCap = StrokeEndLineCap,
	            DashCap = StrokeDashCap,
	            LineJoin = StrokeLineJoin,
	            MiterLimit = StrokeMiterLimit
            };

	        if (StrokeDashArray != null
                || !StrokeDashOffset.IsEqualOrCloseToZero())
            {
                strokePen.DashStyle = new DashStyle(StrokeDashArray, StrokeDashOffset);
            }

			strokePen.Freeze();
            return strokePen;
        }

        private Size GetNaturalSize()
        {
            double strokeThickness = StrokeThickness.Abs();
            return new Size(strokeThickness, strokeThickness);
		}

		#endregion Rendering Resources
	}
}

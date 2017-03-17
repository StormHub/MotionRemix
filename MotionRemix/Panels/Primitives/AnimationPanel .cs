using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MotionRemix.Common;

namespace MotionRemix.Panels.Primitives
{
	/// <summary>
	/// Provides panel animation support.
	/// </summary>
	public abstract class AnimationPanel : Panel
	{
		#region Fields

		private const double MinimumValueDelta = 0.1;
		private const double TerminalVelocity = 10000;

		private readonly CompositionTargetRenderingListener renderingListener;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="AnimationPanel"/>.
		/// </summary>
		protected AnimationPanel()
		{
			renderingListener = new CompositionTargetRenderingListener();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		#endregion Constructors

		#region Types

		/// <summary>
		/// Provides item animation information.
		/// </summary>
		private class ItemAnimationData
		{
			#region Fields

			private static readonly Random Random = new Random();

			private readonly double randomSeed;
			private readonly TranslateTransform transform;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// Initializes a new instance of <see cref="AnimationPanel.ItemAnimationData"/>.
			/// </summary>
			public ItemAnimationData()
			{
				randomSeed = Random.NextDouble();
				transform = new TranslateTransform();
			}

			#endregion Constructors

			#region Properties

			/// <summary>
			/// Target location.
			/// </summary>
			public Point Target
			{
				get;
				set;
			}

			/// <summary>
			/// Current location.
			/// </summary>
			public Point Current
			{
				get;
				set;
			}

			/// <summary>
			/// Gets the offset from target to current.
			/// </summary>
			public Vector Offset
			{
				get
				{
					return Target - Current;
				}
			}

			/// <summary>
			/// Location velocity.
			/// </summary>
			public Vector LocationVelocity
			{
				get;
				set;
			}

			/// <summary>
			/// Radom seed.
			/// </summary>
			public double RandomSeed
			{
				get
				{
					return randomSeed;
				}
			}

			/// <summary>
			/// The <see cref="TranslateTransform"/>.
			/// </summary>
			public TranslateTransform Transform
			{
				get
				{
					return transform;
				}
			}

			#endregion Properties
		}

		#endregion Types

		#region Dependency Properties

		#region Dampening

		private const string DampeningPropertyName = "Dampening";

		/// <summary>
		/// Dependency property for Dampening.
		/// </summary>
		public static readonly DependencyProperty DampeningProperty =
			DependencyProperty.Register(
				DampeningPropertyName,
				typeof(double),
				typeof(AnimationPanel),
				new FrameworkPropertyMetadata(0.2),
				IsValidDampening);

		/// <summary>
		/// Dampening property
		/// </summary>
		public double Dampening
		{
			get
			{
				return (double)GetValue(DampeningProperty);
			}
			set
			{
				SetValue(DampeningProperty, value);
			}
		}

		private static bool IsValidDampening(object value)
		{
			double target = (double)value;
			return target.IsGreaterThanOrCloseTo(0) && target.IsLessThanOrCloseTo(1);
		}

		#endregion Dampening

		#region Attraction

		private const string AttractionPropertyName = "Attraction";

		/// <summary>
		/// Dependency property for Dampening.
		/// </summary>
		public static readonly DependencyProperty AttractionProperty =
			DependencyProperty.Register(
				AttractionPropertyName,
				typeof(double),
				typeof(AnimationPanel),
				new FrameworkPropertyMetadata(2d),
				IsValidAttraction);

		/// <summary>
		/// Attraction property.
		/// </summary>
		public double Attraction
		{
			get
			{
				return (double)GetValue(AttractionProperty);
			}
			set
			{
				SetValue(AttractionProperty, value);
			}
		}

		private static bool IsValidAttraction(object value)
		{
			double target = (double)value;
			return target.IsGreaterThanOrCloseTo(0);
		}

		#endregion Attraction

		#region Variation

		private const string VariationPropertyName = "Variation";

		/// <summary>
		/// Dependency property for Variation.
		/// </summary>
		public static readonly DependencyProperty VariationProperty =
			DependencyProperty.Register(
				VariationPropertyName,
				typeof(double),
				typeof(AnimationPanel),
				new FrameworkPropertyMetadata(1d),
				IsValidVariation);

		/// <summary>
		/// Variation property.
		/// </summary>
		public double Variation
		{
			get
			{
				return (double)GetValue(VariationProperty);
			}
			set
			{
				SetValue(VariationProperty, value);
			}
		}

		private static bool IsValidVariation(object value)
		{
			double target = (double)value;
			return target.IsGreaterThanOrCloseTo(0) && target.IsLessThanOrCloseTo(1);
		}

		#endregion Variation

		#region AnimationData

		private const string AnimationDataPropertyName = "AnimationData";

		/// <summary>
		/// Attached property for item animations.
		/// </summary>
		private static readonly DependencyProperty AnimationDataProperty =
			DependencyProperty.RegisterAttached(
				AnimationDataPropertyName,
				typeof(ItemAnimationData),
				typeof(AnimationPanel));

		private static ItemAnimationData GetAnimationData(DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}

			return (ItemAnimationData)element.GetValue(AnimationDataProperty);
		}

		private static void SetAnimationData(DependencyObject element, ItemAnimationData value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			element.SetValue(AnimationDataProperty, value);
		}

		#endregion AnimationData

		#endregion Dependency Properties

		#region Overrides

		/// <summary>
		/// Invoked when the System.Windows.Media.VisualCollection of a visual object
		/// is modified.
		/// </summary>
		/// <param name="visualAdded">
		/// The System.Windows.Media.Visual that was added to the collection.
		/// </param>
		/// <param name="visualRemoved">
		///     The System.Windows.Media.Visual that was removed from the collection.
		/// </param>
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			if (visualRemoved == null)
			{
				return;
			}

			ItemAnimationData animationData = GetAnimationData(visualRemoved);
			if (animationData == null)
			{
				return;
			}

			visualRemoved.ClearValue(AnimationDataProperty);
		}

		#endregion Overrides

		#region Arrange Elements

		/// <summary>
		/// Arranges the <see cref="UIElement"/> for hte specified <see cref="Rect"/>
		/// bounds.
		/// </summary>
		/// <param name="element">
		/// The <see cref="UIElement"/> to arrange.
		/// </param>
		/// <param name="bounds">
		/// The <see cref="Rect"/> bound for the element.
		/// </param>
		protected void ArrangeElement(UIElement element, Rect bounds)
		{
			if (element == null)
			{
				return;
			}

			if (!renderingListener.IsListening)
			{
				renderingListener.StartListening();
			}

			ItemAnimationData animationData = GetAnimationData(element);
			if (animationData == null)
			{
				animationData = new ItemAnimationData();
				SetAnimationData(element, animationData);

				element.RenderTransform = animationData.Transform;
				animationData.Current = OnElementAdded(element, bounds);
			}
			animationData.Target = bounds.Location;

			element.Arrange(bounds);
		}

		/// <summary>
		/// Invoked when a new <see cref="UIElement"/> is added.
		/// </summary>
		/// <param name="element">
		/// The <see cref="UIElement"/> to add.
		/// </param>
		/// <param name="bounds">
		/// The element <see cref="Rect"/> bound.
		/// </param>
		/// <returns>
		/// The element location.
		/// </returns>
		protected virtual Point OnElementAdded(UIElement element, Rect bounds)
		{
			return bounds.Location;
		}

		#endregion Arrange Elements

		#region Rendering

		private void OnRendering(object sender, EventArgs eventArgs)
		{
			double dampening = Dampening;
			double attractionFactor = Attraction * .01;
			double variation = Variation;

			bool hasChanges = false;
			for (int i = 0; i < InternalChildren.Count; i++)
			{
				ItemAnimationData animationData = GetAnimationData(Children[i]);
				if (animationData != null)
				{
					attractionFactor *= 1 + (variation * animationData.RandomSeed - .5);
					Vector currentVelocity = animationData.LocationVelocity;

					Point newLocation;
					Vector newVelocity;

					Vector diff = animationData.Offset;
					if (diff.Length > MinimumValueDelta || currentVelocity.Length > MinimumValueDelta)
					{
						newVelocity = currentVelocity * (1 - dampening);
						newVelocity += diff * attractionFactor;
						if (currentVelocity.Length > TerminalVelocity)
						{
							newVelocity *= TerminalVelocity / currentVelocity.Length;
						}

						newLocation = animationData.Current + newVelocity;
						hasChanges = true;
					}
					else
					{
						newLocation = animationData.Target;
						newVelocity = new Vector();
					}

					animationData.Current = newLocation;
					animationData.LocationVelocity = newVelocity;

					Vector transformVector = animationData.Current - animationData.Target;
					animationData.Transform.X = transformVector.X;
					animationData.Transform.Y = transformVector.Y;
				}
			}

			// Stop listening if no changes for all elements
			if (!hasChanges)
			{
				renderingListener.StopListening();
			}
		}

		#endregion Rendering

		#region Event Handlers

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			renderingListener.Rendering += OnRendering;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			renderingListener.StopListening();
			renderingListener.Rendering -= OnRendering;
		}

		#endregion Event Handlers
	}
}

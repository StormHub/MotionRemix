using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MotionRemix.Common;

namespace MotionRemix.Panels.Primitives
{
    /// <summary>
    /// Decorator to present item transition animations.
    /// </summary>
    public class TileDecorator : Decorator
    {
        #region Constants

        private const double AttractionFator = .05;
        private const double Dampening = .3;
        private const double TerminalVelocity = .1;
        private const double MinimalValueDelta = .00001;
        private const double MinimalVelocityDelta = .00001;

        #endregion Constants

        #region Fields

        private readonly CompositionTargetRenderingListener listener;
        private readonly TranslateTransform traslateTransform;

        private ActiveTilePanel tilePanel;

        private double percentOffset;
        private double velocity;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="TileDecorator"/>.
        /// </summary>
        public TileDecorator()
        {
            listener = new CompositionTargetRenderingListener();
            listener.Rendering += OnRendering;

            traslateTransform = new TranslateTransform();
        }

        #endregion Constructors

        #region Dependency Properties

        #region TargetIndex

        private const string TargetIndexPropertyName = "TargetIndex";

        /// <summary>
        /// Dependency property for TargetIndex.
        /// </summary>
        public static readonly DependencyProperty TargetIndexProperty =
            DependencyProperty.Register(
                TargetIndexPropertyName,
                typeof(int),
                typeof(TileDecorator),
                new FrameworkPropertyMetadata(
                    0,
                    OnTargetIndexPropertyChanged));

        /// <summary>
        /// Gets or sets the TargetIndex.
        /// </summary>
        public int TargetIndex
        {
            get
            {
                return (int)GetValue(TargetIndexProperty);
            }
            set
            {
                SetValue(TargetIndexProperty, value);
            }
        }

        private static void OnTargetIndexPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            TileDecorator target = element as TileDecorator;
            if (target == null)
            {
                return;
            }

            target.listener.StartListening();
        }

        #endregion TargetIndex

        #endregion Dependency Properties

        #region Rendering

        /// <summary>
        /// Measures all child sizes for the specified <see cref="Size"/>.
        /// </summary>
        /// <param name="availableSize">
        /// The available size.
        /// </param>
        /// <returns>
        /// The measured size for all child items.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size();

            UIElement child = Child;
            if (child != null)
            {
                listener.StartListening();
                child.Measure(availableSize);
            }

            return size;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            ActiveTilePanel panel = Child as ActiveTilePanel;
            if (panel == null)
            {
                return;
            }
            if (!ReferenceEquals(tilePanel, panel))
            {
                tilePanel = panel;
                tilePanel.RenderTransform = traslateTransform;
            }

            int actualTargetIndex = Math.Max(0, Math.Min(tilePanel.Children.Count - 1, TargetIndex));

            double targetPercentOffset = -actualTargetIndex / (double)tilePanel.Children.Count;
            targetPercentOffset = targetPercentOffset.IsNaNValue() ? 0 : targetPercentOffset;

            bool stopListening = !Animate(targetPercentOffset);

            double targetPixelOffset = percentOffset * (RenderSize.Width * tilePanel.Children.Count);
            traslateTransform.X = targetPixelOffset;

            if (stopListening)
            {
                listener.StopListening();
            }
        }

        private bool Animate(double targetValue)
        {
            double diff = targetValue - percentOffset;

            if (diff.Abs() > MinimalValueDelta
                || velocity.Abs() > MinimalVelocityDelta)
            {
                velocity = velocity * (1 - Dampening);
                velocity += diff * AttractionFator;
                if (velocity.Abs() > TerminalVelocity)
                {
                    velocity *= TerminalVelocity / velocity.Abs();
                }

                percentOffset = percentOffset + velocity;

                return true;
            }

            percentOffset = targetValue;
            velocity = 0;
            return false;
        }

        #endregion Rendering
    }
}

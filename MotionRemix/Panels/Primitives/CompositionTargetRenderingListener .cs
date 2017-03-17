using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace MotionRemix.Panels.Primitives
{
	/// <summary>
	/// Provides composition rendering for animations.
	/// </summary>
	public class CompositionTargetRenderingListener : DispatcherObject, IDisposable
	{
		#region Fields

		private bool isListening;
		private bool isDisposed;
		private int tickCount;
		private int lastTick;

		#endregion Fields

		#region Events

		/// <summary>
		/// Raised when the IsListening property is changed.
		/// </summary>
		public event EventHandler IsListeningChanged;

		/// <summary>
		/// Raised when the composition target is rendering
		/// </summary>
		public event EventHandler Rendering;

		#endregion Events

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="CompositionTargetRenderingListener"/>.
		/// </summary>
		public CompositionTargetRenderingListener()
		{
			isListening = false;
			isDisposed = false;
			tickCount = 0;
			lastTick = 0;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Indicates whether is listening to the composite target or not.
		/// </summary>
		public bool IsListening
		{
			get
			{
				return isListening;
			}
			private set
			{
				if (isListening == value)
				{
					return;
				}

				isListening = value;
				OnIsListeneningChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Inicates whether the object is disposed or not.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				VerifyRequiredAccess();
				return isDisposed;
			}
		}

		/// <summary>
		/// The tick count since the last time.
		/// </summary>
		public int TickCount
		{
			get
			{
				return tickCount;
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Starts to listening to rendering events.
		/// </summary>
		public void StartListening()
		{
			if (isListening)
			{
				return;
			}

			VerifyRequiredAccess();
			IsListening = true;
			lastTick = Environment.TickCount;
			tickCount = 0;
			CompositionTarget.Rendering += OnCompositionTargetRendering;
		}

		/// <summary>
		/// Stops listening to rendering events.
		/// </summary>
		public void StopListening()
		{
			if (!isListening)
			{
				return;
			}

			VerifyRequiredAccess();
			IsListening = false;
			CompositionTarget.Rendering -= OnCompositionTargetRendering;
			lastTick = 0;
			tickCount = 0;
		}

		private void OnIsListeneningChanged(EventArgs args)
		{
			if (IsListeningChanged == null)
			{
				return;
			}

			IsListeningChanged(this, args);
		}

		private void OnCompositionTargetRendering(object sender, EventArgs e)
		{
			OnRendering(e);
		}

		protected virtual void OnRendering(EventArgs args)
		{
			if (Rendering == null)
			{
				return;
			}

			int tick = Environment.TickCount;
			tickCount = tick - lastTick;
			lastTick = tick;

			VerifyRequiredAccess();
			Rendering(this, args);
		}

		private void VerifyRequiredAccess()
		{
			VerifyAccess();

			if (isDisposed)
			{
				throw new ObjectDisposedException("CompositionTargetRenderingListener");
			}
		}

		/// <summary>
		/// Disposes the current instance.
		/// </summary>
		public void Dispose()
		{
			StopListening();

			if (Rendering != null)
			{
				Delegate[] list = Rendering.GetInvocationList();
				foreach (Delegate handler in list)
				{
					Rendering -= (EventHandler)handler;
				}
			}

			isDisposed = true;
		}

		#endregion Methods
	}
}

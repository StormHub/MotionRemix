using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MotionRemix.Demo.ViewModels;

namespace MotionRemix.Demo.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string MinimizeButtonTemplatPartName = "PART_MinimizeButton";
		private const string MaximizeButtonTemplatPartName = "PART_MaximizeButton";
		private const string CloseButtonTemplatPartName = "PART_CloseButton";
		private const string MaximizePathTemplatePartName = "PART_MaximizePath";
		private const string RestorePathTemplatePartName = "PART_RestorePath";

		private Button minimizeButton;
		private Button maximizeButton;
		private Button closeButton;
		private Path maximizePath;
		private Path restorePath;

		private readonly MainViewModel mainViewModel;

		/// <summary>
		/// Initializes a new instance of <see cref="MainWindow"/>.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			mainViewModel = new MainViewModel();
			DataContext = mainViewModel;

			StateChanged += OnWindowStateChanged;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			minimizeButton = GetTemplateChild(MinimizeButtonTemplatPartName) as Button;
			if (minimizeButton != null)
			{
				minimizeButton.Click += OnMinimizeButtonClick;
			}

			maximizeButton = GetTemplateChild(MaximizeButtonTemplatPartName) as Button;
			if (maximizeButton != null)
			{
				maximizeButton.Click += OnMaximizeButtonClick;
			}

			closeButton = GetTemplateChild(CloseButtonTemplatPartName) as Button;
			if (closeButton != null)
			{
				closeButton.Click += OnCloseButtonClick;
			}

			maximizePath = GetTemplateChild(MaximizePathTemplatePartName) as Path;
			restorePath = GetTemplateChild(RestorePathTemplatePartName) as Path;
		}

		private void OnWindowStateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Normal)
			{
				if (maximizeButton != null)
				{
					maximizeButton.ToolTip = "Maximize";
				}

				if (maximizePath != null)
				{
					maximizePath.Visibility = Visibility.Visible;
				}

				if (restorePath != null)
				{
					restorePath.Visibility = Visibility.Collapsed;
				}
			}

			if (WindowState == WindowState.Maximized)
			{
				if (maximizeButton != null)
				{
					maximizeButton.ToolTip = "Restore";
				}

				if (maximizePath != null)
				{
					maximizePath.Visibility = Visibility.Collapsed;
				}

				if (restorePath != null)
				{
					restorePath.Visibility = Visibility.Visible;
				}
			}
		}

		private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}

		private void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}

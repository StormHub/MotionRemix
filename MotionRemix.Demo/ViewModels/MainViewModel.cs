

namespace MotionRemix.Demo.ViewModels
{
	/// <summary>
	/// Main view model for the main window.
	/// </summary>
    public class MainViewModel : ViewModelBase
	{
        private readonly AnimationListViewModel animationListViewModel;
        private readonly EasingListViewModel easingListViewModel;

		/// <summary>
		/// Initializes a new instance of <see cref="MainViewModel"/>
		/// </summary>
		public MainViewModel()
		{
            animationListViewModel = new AnimationListViewModel();
            easingListViewModel = new EasingListViewModel();
		}

        /// <summary>
        /// Gets the animation list view model.
        /// </summary>
        public AnimationListViewModel AnimationList
        {
            get
            {
                return animationListViewModel;
            }
        }

        /// <summary>
        /// Gets the easing list view model.
        /// </summary>
        public EasingListViewModel EasingList
        {
            get
            {
                return easingListViewModel;
            }
        }
	}
}

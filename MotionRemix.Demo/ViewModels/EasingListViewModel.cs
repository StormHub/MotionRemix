using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model for easing functions.
    /// </summary>
    public class EasingListViewModel : ViewModelBase
    {
        private readonly ReadOnlyCollection<ViewModelBase> easingList;

        /// <summary>
        /// Initializes a new instance of <see cref="EasingListViewModel"/>.
        /// </summary>
        public EasingListViewModel()
        {
            List<ViewModelBase> viewModels = new List<ViewModelBase>
            {
                new CubicBezierEaseViewModel(),
                new QuadraticBezierEaseViewModel()
            };

            easingList = new ReadOnlyCollection<ViewModelBase>(viewModels);
        }

        /// <summary>
        /// A list of easing view models.
        /// </summary>
        public IReadOnlyCollection<ViewModelBase> EasingList
        {
            get
            {
                return easingList;
            }
        }
    }
}

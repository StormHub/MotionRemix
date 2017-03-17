using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model for animation list;
    /// </summary>
    public class AnimationListViewModel : ViewModelBase
    {
        private readonly ReadOnlyCollection<ViewModelBase> animationList;

        /// <summary>
        /// Initializes a new instance of <see cref="AnimationListViewModel"/>.
        /// </summary>
        public AnimationListViewModel()
        {
            List<ViewModelBase> viewModels = new List<ViewModelBase>
            {
                new ColorUsingHsbViewModel(),
                new GradientBrushOffsetViewModel(),
                new GradientBrushFlowViewModel(),
                new GradientBrushWaveViewModel()
            };

            animationList = new ReadOnlyCollection<ViewModelBase>(viewModels);
        }

        /// <summary>
        /// A list of animation view models.
        /// </summary>
        public IReadOnlyCollection<ViewModelBase> AnimationList
        {
            get
            {
                return animationList;
            }
        }
    }
}

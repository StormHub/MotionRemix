using System.ComponentModel;

namespace MotionRemix.Demo.ViewModels
{
    /// <summary>
    /// View model base class.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Property change notification event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised the property change event for the specified property name.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || PropertyChanged == null)
            {
                return;
            }

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

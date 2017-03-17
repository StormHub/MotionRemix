using System;
using System.Globalization;
using System.Windows.Data;

namespace MotionRemix.Converters
{
    /// <summary>
    /// Converts boolean value to corresponding target type values.
    /// </summary>
    /// <typeparam name="T">
    /// The type value type.
    /// </typeparam>
    public class BooleanConverter<T> : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of BooleanConverter.
        /// </summary>
        /// <param name="trueValue">
        /// Value to return if true.
        /// </param>
        /// <param name="falseValue">
        /// Value to return if false.
        /// </param>
        public BooleanConverter(T trueValue, T falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        /// <summary>
        /// Target value for true.
        /// </summary>
        public T TrueValue
        {
            get;
            set;
        }

        /// <summary>
        /// Target value for false.
        /// </summary>
        public T FalseValue
        {
            get;
            set;
        }

        /// <summary>
        /// Converts the time duration to display string in English.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="targetType">
        /// The target type to convert to.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> instance.
        /// </param>
        /// <returns>
        /// The display string for the duration.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result;

            if (value is bool)
            {
                result = (bool)value;
            }
            else
            {
                bool? nullable = (bool?)value;
                result = nullable.HasValue && nullable.Value;
            }

            return result ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts the target value back to boolean, not supported.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="targetType">
        /// The target type to convert to.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> instance.
        /// </param>
        /// <returns>
        /// unset dependency value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is T)
            {
                return Equals(TrueValue, value);
            }

            return false;
        }
    }
}

using System;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace MotionRemix.Common
{
    /// <summary>
    /// Provides HSB (hue, Saturation, Brightness) representation of colors.
    /// </summary>
    public struct HsbColor : IFormattable, IEquatable<HsbColor>
    {
        #region Fields

        private readonly float hue;
        private readonly float saturation;
        private readonly float brightness;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="HsbColor"/> for the
        /// specified <see cref="Color"/> instance.
        /// </summary>
        /// <param name="color">
        /// The <see cref="Color"/> instance.
        /// </param>
        public HsbColor(Color color)
        {
            float red = color.R / (float)byte.MaxValue;
            float green = color.G / (float)byte.MaxValue;
            float blue = color.B / (float)byte.MaxValue;

            float minimum = Math.Min(red, Math.Min(green, blue));
            float maximum = Math.Max(red, Math.Max(green, blue));
            float delta = maximum - minimum;

            brightness = maximum;

            // Calculate the hue (in degrees of a circle, between 0 and 360)
            if (maximum.IsCloseTo(red))
            {
                if (green.IsGreaterThanOrCloseTo(blue))
                {
                    hue = delta.IsZero() 
                        ? 0 
                        : 60 * (green - blue) / delta;
                }
                else
                {
                    hue = 60 * (green - blue) / delta + 360;
                }
            }
            else if (maximum.IsCloseTo(green))
            {
                hue = 60 * (blue - red) / delta + 120;
            }
            else // maximum.IsCloseTo(blue)
            {
                hue = 60 * (red - green) / delta + 240;
            }

            //Calculate the saturation (between 0 and 1)
            saturation = maximum.IsZero() 
                ? 0 
                : 1 - (minimum / maximum);

            //Scale the saturation and value to a percentage between 0 and 100
            saturation *= 100;
            brightness *= 100;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HsbColor"/> for the specified hue,
        /// saturation and brightness.
        /// </summary>
        /// <param name="hue">
        /// The hue value.
        /// </param>
        /// <param name="saturation">
        /// The saturation value.
        /// </param>
        /// <param name="brightness">
        /// The brightness value.
        /// </param>
        public HsbColor(float hue, float saturation, float brightness)
        {
            if (!hue.IsBetween(0, 360))
            {
                throw new ArgumentException("Hue must be between 0 to 360 inclusive.", "hue");
            }
            if (!saturation.IsBetween(0, 100))
            {
                throw new ArgumentException("Saturation must be between 0 to 100 inclusive.", "saturation");
            }
            if (!brightness.IsBetween(0, 100))
            {
                throw new ArgumentException("Brightness must be between 0 to 100 inclusive.", "brightness");
            }

            this.hue = hue;
            this.saturation = saturation;
            this.brightness = brightness;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Hue for HSB color mode.
        /// </summary>
        public float Hue
        {
            get
            {
                return hue;
            }
        }

        /// <summary>
        /// Saturation for HSB color mode.
        /// </summary>
        public float Saturation
        {
            get
            {
                return saturation;
            }
        }

        /// <summary>
        /// Brightness for HSB color mode.
        /// </summary>
        public float Brightness
        {
            get
            {
                return brightness;
            }
        }

        #endregion Properties

        #region Conversion

        /// <summary>
        /// Offsets the specified <see cref="Color"/> by a set of HSB (hue, Saturation, Brightness) 
        /// values.
        /// </summary>
        /// <param name="color">
        /// The from <see cref="Color"/>.
        /// </param>
        /// <param name="hueOffset">
        /// The hue offset.
        /// </param>
        /// <param name="saturationOffset">
        /// The staturation offset.
        /// </param>
        /// <param name="brightnessOffset">
        /// The brightness offset.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/> offsets form <paramref name="color"/> by a set of HSB 
        /// (hue, Saturation, Brightness) values.
        /// </returns>
        public static Color Offset(
            Color color, 
            float hueOffset, 
            float saturationOffset, 
            float brightnessOffset)
        {
            HsbColor hsbColor = new HsbColor(color);

            // Normalize all HSB values
            if (hueOffset.Abs() > 360)
            {
                hueOffset %= 360;
            }
            if (hueOffset.IsLessThan(0))
            {
                hueOffset += 360;
            }

            if (saturationOffset.Abs() > 100)
            {
                saturationOffset %= 100;
            }
            if (saturationOffset.IsLessThan(0))
            {
                saturationOffset += 100;
            }

            if (brightnessOffset.Abs() > 100)
            {
                brightnessOffset %= 100;
            }
            if (brightnessOffset.IsLessThan(0))
            {
                brightnessOffset += 100;
            }

            HsbColor destinationColor = hsbColor.Offset(
                hueOffset, 
                saturationOffset, 
                brightnessOffset);

            return (Color)destinationColor;
        }

        /// <summary>
        /// Offsets the current instance by the specified hue, saturation and brightness offset.
        /// </summary>
        /// <param name="hueOffset">
        /// The hue offset.
        /// </param>
        /// <param name="saturationOffset">
        /// The staturation offset.
        /// </param>
        /// <param name="brightnessOffset">
        /// The brightness offset.
        /// </param>
        /// <returns>
        /// A new <see cref="HsbColor"/> by the specified hue, saturation and brightness offset.
        /// </returns>
        public HsbColor Offset(
            float hueOffset, 
            float saturationOffset, 
            float brightnessOffset)
        {
            if (!hueOffset.IsBetween(0, 360))
            {
                throw new ArgumentException("Hue offset must be between 0 to 360 inclusive.", "hueOffset");
            }
            if (!saturationOffset.IsBetween(0, 100))
            {
                throw new ArgumentException("Saturation offset must be between 0 to 100 inclusive.", "saturationOffset");
            }
            if (!brightnessOffset.IsBetween(0, 100))
            {
                throw new ArgumentException("Brightness offset must be between 0 to 100 inclusive.", "brightnessOffset");
            }

            float hueValue = hue + hueOffset;
            if (hueValue.IsGreaterThan(360))
            {
                hueValue %= 360;
            }

            float saturationValue = saturation + saturationOffset;
            if (saturationValue.IsGreaterThan(100))
            {
                saturationValue %= 100;
            }

            float brightnessValue = brightness + brightnessOffset;
            if (brightnessValue.IsGreaterThan(100))
            {
                brightnessValue %= 100;
            }

            return new HsbColor(hueValue, saturationValue, brightnessValue);
        }

        /// <summary>
        /// Explicit conversion to <see cref="Color"/>.
        /// </summary>
        /// <param name="hsbColor">
        /// The <see cref="HsbColor"/> to convert.
        /// </param> 
        /// <returns> 
        /// The <see cref="Color"/> representation.
        /// </returns>
        public static explicit operator Color(HsbColor hsbColor)
        {
            float hue = hsbColor.hue;
            float saturation = hsbColor.saturation / 100;
            float brightness = hsbColor.brightness / 100;

            float red = 0;
            float green = 0;
            float blue = 0;

            if (saturation.IsZero())
            {
                // If the saturation is 0, then all colors are the same
                // This is some flavor of gray.
                red = green = blue = brightness;
            }
            else
            {
                // Calculate the appropriate sector of a 6-part color wheel
                float sectorPosition = hue / 60;
                int sectorNumber = (int)Math.Floor(sectorPosition);

                // Get the fractional part of the sector
                // that is, how many degrees into the sector you are
                float fractionalSector = sectorPosition - sectorNumber;

                // Calculate values for the three axes of the color
                float p = brightness * (1 - saturation);
                float q = brightness * (1 - saturation * fractionalSector);
                float t = brightness * (1 - saturation * (1 - fractionalSector));

                // Assign the fractional colors to red, green, and blue
                // components based on the sector the angle is in
                switch (sectorNumber)
                {
                    case 0:
                    case 6:
                        {
                            red = brightness;
                            green = t;
                            blue = p;
                            break;
                        }
                    case 1:
                        {
                            red = q;
                            green = brightness;
                            blue = p;
                            break;
                        }
                    case 2:
                        {
                            red = p;
                            green = brightness;
                            blue = t;
                            break;
                        }
                    case 3:
                        {
                            red = p;
                            green = q;
                            blue = brightness;
                            break;
                        }
                    case 4:
                        {
                            red = t;
                            green = p;
                            blue = brightness;
                            break;
                        }
                    case 5:
                        {
                            red = brightness;
                            green = p;
                            blue = q;
                            break;
                        }
                }
            }

            // Scale the red, green, and blue values to be between 0 and 255
            red *= byte.MaxValue;
            green *= byte.MaxValue;
            blue *= byte.MaxValue;

            return Color.FromArgb(
                byte.MaxValue,
                (byte)Math.Round(red, MidpointRounding.AwayFromZero),
                (byte)Math.Round(green, MidpointRounding.AwayFromZero),
                (byte)Math.Round(blue, MidpointRounding.AwayFromZero));
        }

        #endregion Conversion

        #region Equality

        /// <summary>
        /// IsEqual operator - Compares two HsbColors for exact equality.
        /// </summary>
        /// <param name="value1">
        /// The fist <see cref="HsbColor"/> to compare.
        /// </param>
        /// <param name="value2">
        /// The second <see cref="HsbColor"/> to compare.
        /// </param>
        /// <returns>
        /// True if the two values are equal. Otherwise false.
        /// </returns>
        public static bool operator ==(HsbColor value1, HsbColor value2)
        {
            return value1.hue.IsCloseTo(value2.hue) 
                && value1.saturation.IsCloseTo(value2.saturation) 
                && value1.brightness.IsCloseTo(value2.brightness);
        }

        ///<summary>
        /// Not equal operator.
        ///</summary>
        /// <param name="value1">
        /// The fist <see cref="HsbColor"/> to compare.
        /// </param>
        /// <param name="value2">
        /// The second <see cref="HsbColor"/> to compare.
        /// </param>
        /// <returns>
        /// True if the two values are not equal. Otherwise false.
        /// </returns>
        public static bool operator !=(HsbColor value1, HsbColor value2)
        {
            return (!(value1 == value2));
        } 

        /// <summary>
        /// Compares two colors for exact equality.  Note that float values can acquire error 
        /// when operated upon, such that an exact comparison between two values which are logically 
        /// equal may fail. see cref="AreClose" for a "fuzzy" version of this comparison.
        /// </summary> 
        /// <param name='other'>
        /// The color to compare to "this".
        /// </param>
        /// <returns>
        /// Whether or not the two colors are equal
        /// </returns>
        public bool Equals(HsbColor other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares two colors for exact equality.  Note that float values can acquire error 
        /// when operated upon, such that an exact comparison between two vEquals(color);alues which are logically
        /// equal may fail. see cref="AreClose" for a "fuzzy" version of this comparison.
        /// </summary>
        /// <param name='o'>The object to compare to "this"</param> 
        /// <returns>Whether or not the two colors are equal</returns>
        public override bool Equals(object o)
        {
            if (o is HsbColor)
            {
                HsbColor hsbColor = (HsbColor)o;

                return (this == hsbColor);
            }

            return false;
        }

        /// <summary>
        /// IsEqual operator - Compares two HsbColors for exact equality.
        /// </summary>
        /// <param name="value1">
        /// The fist <see cref="HsbColor"/> to compare.
        /// </param>
        /// <param name="value2">
        /// The second <see cref="HsbColor"/> to compare.
        /// </param>
        /// <returns>
        /// True if the two values are equal. Otherwise false.
        /// </returns>
        public static bool Equals(HsbColor value1, HsbColor value2)
        {
            return (value1 == value2);
        }

        ///<summary> 
        /// Gets the has code for this instance.
        ///</summary>
        public override int GetHashCode()
        {
            return hue.GetHashCode() 
                ^ saturation.GetHashCode() 
                ^ brightness.GetHashCode();
        }

        #endregion Equality

        #region String Format

        /// <summary>
        /// Creates a string representation of this object based on the current culture. 
        /// </summary> 
        /// <returns>
        /// A string representation of this object. 
        /// </returns>
        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Creates a string representation of this object based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used. 
        /// </summary>
        /// <returns> 
        /// A string representation of this object. 
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return ToString(null, provider);
        }

        /// <summary> 
        /// Creates a string representation of this object based on the format string 
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used. 
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object. 
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder buffer = new StringBuilder();

            if (string.IsNullOrEmpty(format))
            {
                buffer.AppendFormat(formatProvider, " H:{0} D", hue);
                buffer.AppendFormat(formatProvider, " S:{0} %", saturation);
                buffer.AppendFormat(formatProvider, " B:{0} %", brightness);
            }
            else
            {
                buffer.AppendFormat(formatProvider,
                    "H:{" + format + "}D S:{" + format + "}% B:{" + format + "}%",
                    hue, saturation, brightness); 
            }

            return buffer.ToString();
        }

        #endregion String Format
    }
}

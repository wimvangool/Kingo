using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Windows
{
    /// <summary>
    /// Represents a converter that can convert values to <see cref="Distance"/> instances.
    /// </summary>
    public sealed class DistanceConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return
                sourceType == typeof(string) ||
                sourceType == typeof(double) ||
                base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return Distance.Zero;
            }
            if (value is string)
            {
                return new Distance(double.Parse((string) value, culture));
            }
            if (value is double)
            {
                return new Distance((double) value);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}

using System.Globalization;

namespace System.ComponentModel.FluentValidation
{
    internal sealed class StringToNumberConverter<TValue>
    {
        private readonly TryConvertDelegate<TValue> _tryConvert;
        private TValue _number;

        internal StringToNumberConverter(TryConvertDelegate<TValue> tryConvert)
        {
            _tryConvert = tryConvert;
        }

        internal TValue Number
        {
            get { return _number; }
        }

        internal bool TryConvertToNumber(string value, NumberStyles style, IFormatProvider provider)
        {
            return _tryConvert.Invoke(value, style, provider, out _number);
        }        
    }

    internal delegate bool TryConvertDelegate<TValue>(string value, NumberStyles style, IFormatProvider provider, out TValue number);
}

using System;
using System.Collections.Generic;
using Kingo.Clocks;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    internal static class AggregateRootVersion
    {
        private static readonly Func<byte, byte> _IncrementByte = version => { checked { return (byte) (version + 1); } };
        private static readonly Func<short, short> _IncrementInt16 = version => { checked { return (short) (version + 1); } };
        private static readonly Func<int, int> _IncrementInt32 = version => { checked { return version + 1; } };
        private static readonly Func<long, long> _IncrementInt64 = version => { checked { return version + 1L; } };
        private static readonly Func<DateTimeOffset, DateTimeOffset> _IncrementDateTimeOffset = version => Clock.Current.UtcDateAndTime();
        private static readonly Func<DateTime, DateTime> _IncrementDateTime = version => Clock.Current.UtcDateAndTime().DateTime;
        private static readonly Dictionary<Type, Delegate> _IncrementMethods = new Dictionary<Type, Delegate>
        {
            { typeof(byte), _IncrementByte },
            { typeof(short), _IncrementInt16 },
            { typeof(int), _IncrementInt32 },
            { typeof(long), _IncrementInt64 },
            { typeof(DateTimeOffset), _IncrementDateTimeOffset },
            { typeof(DateTime), _IncrementDateTime }
        };

        internal static Func<TVersion, TVersion> IncrementMethod<TVersion>()
        {
            Delegate method;

            if (_IncrementMethods.TryGetValue(typeof(TVersion), out method))
            {
                return (Func<TVersion, TVersion>) method;
            }
            return version => { throw new InvalidOperationException(ExceptionMessages.Version_IncrementNotSupported); };
        }                
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Resources;

namespace Kingo
{
    /// <summary>
    /// Represents a timespan with a specific start- and end date and/or time.
    /// </summary>
    [Serializable]
    public struct DateTimeSpan : IEquatable<DateTimeSpan>, IFormattable
    {       
        /// <summary>
        /// Represents the maximum span of time (from <see cref="DateTimeOffset.MinValue"/> to <see cref="DateTimeOffset.MaxValue"/>).
        /// </summary>
        public static readonly DateTimeSpan MaxValue = new DateTimeSpan(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);             

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSpan" /> class.
        /// </summary>
        /// <param name="start">Start of this time span.</param>
        /// <param name="duration">Duration of this time span.</param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="duration"/> represents a negative time span, or the end of the resulting time span
        /// exceeds <see cref="DateTimeOffset.MaxValue" />.
        /// </exception>
        public DateTimeSpan(DateTime start, TimeSpan duration) :
            this(new DateTimeOffset(start), duration) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSpan" /> class.
        /// </summary>
        /// <param name="start">Start of this time span.</param>
        /// <param name="duration">Duration of this time span.</param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="duration"/> represents a negative time span, or the end of the resulting time span
        /// exceeds <see cref="DateTimeOffset.MaxValue" />.
        /// </exception>
        public DateTimeSpan(DateTimeOffset start, TimeSpan duration) :
            this(start, start.Add(duration)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSpan" /> class. If <paramref name="start"/> and <paramref name="end"/>
        /// are in a different time-zone, <paramref name="end"/> will be adjusted to the time-zone of <paramref name="start"/>.
        /// </summary>
        /// <param name="start">Start of this time span.</param>
        /// <param name="end">End of this time span.</param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start"/> represents a moment in time after <paramref name="end"/>.
        /// </exception>
        public DateTimeSpan(DateTime start, DateTime end) :
            this(new DateTimeOffset(start), new DateTimeOffset(end)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSpan" /> class. If <paramref name="start"/> and <paramref name="end"/>
        /// are in a different time-zone, <paramref name="end"/> will be adjusted to the time-zone of <paramref name="start"/>.
        /// </summary>
        /// <param name="start">Start of this time span.</param>
        /// <param name="end">End of this time span.</param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="start"/> represents a moment in time after <paramref name="end"/>.
        /// </exception>
        public DateTimeSpan(DateTimeOffset start, DateTimeOffset end)            
        {
            if (IsValidSpan(start, ref end))
            {
                Start = start;
                End = end;
            }
            else
            {
                throw NewInvalidSpanException(start, end);
            }
        }              

        /// <summary>
        /// Start of this time span.
        /// </summary>
        public DateTimeOffset Start
        {
            get;
        }

        /// <summary>
        /// End of this time span.
        /// </summary>
        public DateTimeOffset End
        {
            get;
        }

        private bool EndIsIncluded =>
            Start.Equals(End) || End.Equals(DateTimeOffset.MaxValue);

        private static bool IsValidSpan(DateTimeOffset start, ref DateTimeOffset end) =>
            start <= (end = end.ToOffset(start.Offset)) && end <= DateTimeOffset.MaxValue;

        private static Exception NewInvalidSpanException(DateTimeOffset start, DateTimeOffset end)
        {
            var messageFormat = ExceptionMessages.DateTimeSpan_InvalidTimeSpan;
            var message = string.Format(messageFormat, start, end);
            return new ArgumentOutOfRangeException(message, null as Exception);
        }        

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (obj is DateTimeSpan)
            {
                return Equals((DateTimeSpan) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(DateTimeSpan other) =>
            Start.Equals(other.Start) && End.Equals(other.End);

        /// <inheritdoc />
        public override int GetHashCode() =>
            Start.GetHashCode() ^ End.GetHashCode();

        /// <summary>Determines whether <paramref name="left" /> is equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(DateTimeSpan left, DateTimeSpan right) =>
            left.Equals(right);

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(DateTimeSpan left, DateTimeSpan right) =>
            !left.Equals(right);

        #endregion

        #region [====== Duration ======]

        /// <summary>
        /// Returns the duration of this time span.
        /// </summary>
        public TimeSpan Duration =>
            End.Subtract(Start);

        /// <summary>
        /// Implicitly converts a <see cref="DateTimeSpan" /> to a <see cref="TimeSpan" />, representing its duration.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator TimeSpan(DateTimeSpan value) =>
            value.Duration;

        #endregion

        #region [====== Shift ======]

        /// <summary>
        /// Shifts the entire time span forwards or backwards in time, depending on the specified <paramref name="shift"/> value.
        /// </summary>
        /// <param name="shift">The amount of time to shift this time span.</param>
        /// <returns>The resulting time span.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="shift"/> is not valid because the resulting time span would not be a valid time span.
        /// </exception>
        public DateTimeSpan Shift(TimeSpan shift) =>
            new DateTimeSpan(Start.Add(shift), End.Add(shift));

        /// <summary>
        /// Shifts the start of this time span forwards or backwards in time, depending on the specified <paramref name="shift"/> value.
        /// </summary>
        /// <param name="shift">The amount of time to shift the start value.</param>
        /// <returns>The resulting time span.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="shift"/> is not valid because the resulting time span would not be a valid time span.
        /// </exception>
        public DateTimeSpan ShiftStart(TimeSpan shift) =>
            ShiftStart(Start.Add(shift));

        /// <summary>
        /// Shifts the start of this time span forwards or backwards in time, depending on the specified <paramref name="newStart"/> value.
        /// </summary>
        /// <param name="newStart">The new start value.</param>
        /// <returns>The resulting time span, with the offset equal to the current time span offset.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="newStart"/> is not valid because the resulting time span would not be a valid time span.
        /// </exception>
        public DateTimeSpan ShiftStart(DateTimeOffset newStart) =>
            new DateTimeSpan(newStart.ToOffset(Start.Offset), End);

        /// <summary>
        /// Shifts the end of this time span forwards or backwards in time, depending on the specified <paramref name="shift"/> value.
        /// </summary>
        /// <param name="shift">The amount of time to shift the end value.</param>
        /// <returns>The resulting time span.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="shift"/> is not valid because the resulting time span would not be a valid time span.
        /// </exception>
        public DateTimeSpan ShiftEnd(TimeSpan shift) =>
            ShiftEnd(End.Add(shift));

        /// <summary>
        /// Shifts the end of this time span forwards or backwards in time, depending on the specified <paramref name="newEnd"/> value.
        /// </summary>
        /// <param name="newEnd">The new end value.</param>
        /// <returns>The resulting time span, with the offset equal to the current time span offset.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="newEnd"/> is not valid because the resulting time span would not be a valid time span.
        /// </exception>
        public DateTimeSpan ShiftEnd(DateTimeOffset newEnd) =>
            new DateTimeSpan(Start, newEnd.ToOffset(End.Offset));

        #endregion

        #region [====== Conversion ======]

        /// <summary>
        /// Creates and returns a time span that is identical to this instance, but is represented in times of the currently local time zone.
        /// </summary>
        /// <returns>A new <see cref="DateTimeSpan"/> instance.</returns>
        public DateTimeSpan ToLocalTime() =>
            new DateTimeSpan(Start.ToLocalTime(), End.ToLocalTime());

        /// <summary>
        /// Creates and returns a time span that is identical to this instance, but is represented in UTC time.
        /// </summary>
        /// <returns>A new <see cref="DateTimeSpan"/> instance.</returns>
        public DateTimeSpan ToUniveralTime() =>
            new DateTimeSpan(Start.ToUniversalTime(), End.ToUniversalTime());

        /// <summary>
        /// Creates and returns a time span that is identical to this instance, but is represented in times with the specified <paramref name="offset"/>.
        /// </summary>
        /// <returns>A new <see cref="DateTimeSpan"/> instance.</returns>
        public DateTimeSpan ToOffset(TimeSpan offset) =>
            new DateTimeSpan(Start.ToOffset(offset), End.ToOffset(offset));

        #endregion

        #region [====== Formatting ======]

        /// <inheritdoc />
        public override string ToString() =>
            ToString(null);

        /// <summary>
        /// Formats the current time span in the specified format.
        /// </summary>
        /// <param name="format">
        /// The format string used to format the times of this time span.
        /// </param>
        /// <returns>A formatted representation of this time span.</returns>
        public string ToString(string format) =>
            ToString(format ?? "o", CultureInfo.CurrentCulture);

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider) =>
            ToString(Start.ToString(format, formatProvider), End.ToString(format, formatProvider), EndIsIncluded ? ']' : '>');

        private static string ToString(string start, string end, char closingBracket) =>
            $"[{start}, {end}{closingBracket}";

        #endregion

        #region [====== Contains ======]

        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> is part of this time span.
        /// </summary>
        /// <param name="value">The value to check.</param>        
        /// <returns><c>true</c> if <paramref name="value"/> is part of this time span; otherwise <c>false</c>.</returns>
        public bool Contains(DateTime value) =>
            Contains(new DateTimeOffset(value));

        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> is part of this time span.
        /// </summary>
        /// <param name="value">The value to check.</param>        
        /// <returns><c>true</c> if <paramref name="value"/> is part of this time span; otherwise <c>false</c>.</returns>
        public bool Contains(DateTimeOffset value) =>
            ContainsValue(value.ToOffset(Start.Offset));

        private bool ContainsValue(DateTimeOffset value) =>
            Start <= value && (EndIsIncluded ? value <= End : value < End);

        #endregion

        #region [====== Enumerate ======]  

        /// <summary>
        /// Returns all particular points in time that are part of this time span, where each point in time is separated by the given <paramref name="stepSize" />.
        /// If <paramref name="stepSize"/> represents a positive time span, enumeration will begin at <see cref="Start" />, and all points in time will be in
        /// increasing chronological order. If <paramref name="stepSize" /> represents a negative time span, enumeration will begin at <see cref="End"/>, and all
        /// points in time will be in decreasing chronological order.
        /// </summary>
        /// <param name="stepSize">The step-size of the enumeration.</param>
        /// <returns>A lazy initialized collection of points in time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="stepSize" /> is zero.
        /// </exception>
        public IEnumerable<DateTimeOffset> Enumerate(TimeSpan stepSize)
        {
            if (stepSize > TimeSpan.Zero)
            {
                return EnumerateForward(stepSize);
            }
            if (stepSize < TimeSpan.Zero)
            {
                return EnumerateBackward(stepSize);
            }            
            throw NewTimeSpanZeroException(nameof(stepSize));            
        }        

        private IEnumerable<DateTimeOffset> EnumerateForward(TimeSpan stepSize)
        {
            var value = Start;

            do
            {
                yield return value;
            } while (Add(ref value, stepSize) && value <= End);
        }

        private IEnumerable<DateTimeOffset> EnumerateBackward(TimeSpan stepSize)
        {
            var value = End;

            do
            {
                yield return value;
            } while (Add(ref value, stepSize) && Start <= value);
        }

        private static bool Add(ref DateTimeOffset value, TimeSpan stepSize)
        {
            try
            {
                value = value.Add(stepSize);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }                      

        #endregion

        #region [====== Split ======]

        /// <summary>
        /// Splits this time span into several spans, of which all durations match the specified <paramref name="duration"/>, except
        /// (possibly) the last span, which will have a duration of the remaining span of time. If <paramref name="duration"/> represents
        /// a positive time span, enumeration will begin at <see cref="Start" />, and all spans will be in increasing chronological order.
        /// If <paramref name="duration" /> represents a negative time span, enumeration will begin at <see cref="End"/>, and all
        /// spans will be in decreasing chronological order.
        /// </summary>
        /// <param name="duration">The desired duration of each returned span.</param>
        /// <param name="includeRemainder">
        /// If <c>true</c>, the last element of the returned collection will be the remainder of the split-operation, which may or may not
        /// have a duration of less than the specified <paramref name="duration"/>.
        /// </param>
        /// <returns>A lazy initialized collection of time spans.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="duration"/> is zero.
        /// </exception>
        public IEnumerable<DateTimeSpan> Split(TimeSpan duration, bool includeRemainder = false)
        {
            if (duration > TimeSpan.Zero)
            {
                return SplitForward(duration, includeRemainder);
            }
            if (duration < TimeSpan.Zero)
            {
                return SplitBackward(duration, includeRemainder);
            }
            throw NewTimeSpanZeroException(nameof(duration));
        }

        private IEnumerable<DateTimeSpan> SplitForward(TimeSpan duration, bool includeRemainder)
        {
            var span = new DateTimeSpan(Start, TimeSpan.Zero);

            while (AddForward(ref span, duration, End))
            {
                yield return span;
            }
            if (includeRemainder || IsLastElement(span.End, End, duration))
            {
                yield return new DateTimeSpan(span.End, End);
            }
        }        

        private static bool AddForward(ref DateTimeSpan span, TimeSpan duration, DateTimeOffset end)
        {
            var startOfNextSpan = span.End;            
            var endOfNextSpan = span.End;

            if (Add(ref endOfNextSpan, duration) && endOfNextSpan < end)
            {
                span = new DateTimeSpan(startOfNextSpan, endOfNextSpan);
                return true;
            }
            return false;         
        }

        private IEnumerable<DateTimeSpan> SplitBackward(TimeSpan duration, bool includeRemainder)
        {
            var span = new DateTimeSpan(End, TimeSpan.Zero);

            while (AddBackward(ref span, duration, Start))
            {
                yield return span;
            }
            if (includeRemainder || IsLastElement(span.Start, Start, duration))
            {
                yield return new DateTimeSpan(Start, span.Start);
            }
        }

        private static bool AddBackward(ref DateTimeSpan span, TimeSpan duration, DateTimeOffset start)
        {
            var startOfNextSpan = span.Start;
            var endOfNextSpan = span.Start;

            if (Add(ref startOfNextSpan, duration) && start < startOfNextSpan)
            {
                span = new DateTimeSpan(startOfNextSpan, endOfNextSpan);
                return true;
            }
            return false;
        }

        private static bool IsLastElement(DateTimeOffset start, DateTimeOffset end, TimeSpan duration) =>
            end.Subtract(start).Equals(duration);

        #endregion

        #region [====== Intersect ======]

        /// <summary>
        /// Determines whether or not this time span intersects with <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Another time span.</param>
        /// <returns>
        /// <c>true</c> if this time span intersects with <paramref name="other"/>; otherwise <c>false</c>.
        /// </returns>
        public bool IntersectsWith(DateTimeSpan other) =>
            GetIntersection(other).HasValue;

        /// <summary>
        /// Creates and returns a time span that represents the intersection of this time span and <paramref name="other"/>, if it exists.
        /// </summary>
        /// <param name="other">Another time span.</param>
        /// <returns>
        /// The calculated intersection, or <c>null</c> if no intersection exists.
        /// </returns>
        public DateTimeSpan? GetIntersection(DateTimeSpan other)
        {
            if (TryGetIntersection(other, out DateTimeSpan intersection))
            {
                return intersection;
            }
            return null;
        }

        /// <summary>
        /// Creates and returns a time span that represents the intersection of this time span and <paramref name="other"/>, if it exists.
        /// </summary>
        /// <param name="other">Another time span.</param>
        /// <param name="intersection">
        /// If an intersection exists, this parameter will refer to the calculated intersection.
        /// </param>
        /// <returns>
        /// <c>true</c> if this time span intersects with <paramref name="other"/>; otherwise <c>false</c>.
        /// </returns>
        public bool TryGetIntersection(DateTimeSpan other, out DateTimeSpan intersection) =>
            TryGetIntersectionWith(other.ToOffset(Start.Offset), out intersection);

        private bool TryGetIntersectionWith(DateTimeSpan other, out DateTimeSpan intersection)
        {
            var intersectionStart = Max(Start, other.Start);
            var intersectionEnd = Min(End, other.End);

            if (intersectionStart <= intersectionEnd)
            {
                intersection = new DateTimeSpan(intersectionStart, intersectionEnd);
                return true;
            }
            intersection = default(DateTimeSpan);
            return false;
        }

        #endregion

        #region [====== Difference ======]

        /// <summary>
        /// Returns a collection of time spans that represent the differences or non-intersecting spans between this time span and <paramref name="other"/>.
        /// The resulting collection may contain, zero, one or two span, depending on if and how this time span intersects with <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IEnumerable<DateTimeSpan> GetDifference(DateTimeSpan other)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static DateTimeOffset Min(DateTimeOffset left, DateTimeOffset right) =>
            left < right ? left : right;

        private static DateTimeOffset Max(DateTimeOffset left, DateTimeOffset right) =>
            left < right ? right : left;

        private static Exception NewTimeSpanZeroException(string paramName)
        {
            var messageFormat = ExceptionMessages.DateTimeSpan_TimeSpanZeroNotAllowed;
            var message = string.Format(messageFormat, paramName);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #region [====== Factory Methods ======]

        /// <summary>
        /// Creates and returns a new time span that represents the specified <paramref name="year"/> in the specified time zone.
        /// </summary>
        /// <param name="year">A year.</param>
        /// <param name="offset">Offset representing the desired time zone.</param>
        /// <returns>A time span representing the specified <paramref name="year"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="year"/> is a not a valid year.
        /// </exception>
        public static DateTimeSpan FromYear(int year, TimeSpan? offset = null) =>
            FromYear(year, ValueOrDefault(offset));

        private static DateTimeSpan FromYear(int year, TimeSpan offset) =>
            FromYearStart(new DateTimeOffset(year, 1, 1, 0, 0, 0, offset));

        private static DateTimeSpan FromYearStart(DateTimeOffset start) =>
            new DateTimeSpan(start, start.AddYears(1));

        /// <summary>
        /// Creates and returns a new time span that represents the specified <paramref name="month"/> of the specified <paramref name="year"/> in the specified time zone.
        /// </summary>
        /// <param name="year">A year.</param>
        /// <param name="month">A month.</param>
        /// <param name="offset">Offset representing the desired time zone.</param>
        /// <returns>A time span representing the specified month>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="year"/> is a not a valid year or <paramref name="month"/> is not a valid month.
        /// </exception>
        public static DateTimeSpan FromMonth(int year, int month, TimeSpan? offset = null) =>
            FromMonth(year, month, ValueOrDefault(offset));

        private static DateTimeSpan FromMonth(int year, int month, TimeSpan offset) =>
            FromMonthStart(new DateTimeOffset(year, month, 1, 0, 0, 0, offset));

        private static DateTimeSpan FromMonthStart(DateTimeOffset start) =>
            new DateTimeSpan(start, start.AddMonths(1));

        /// <summary>
        /// Creates and returns a new time span that represents the current day (in local time).
        /// </summary>
        /// <returns>A time span representing today.</returns>
        public static DateTimeSpan Today() =>
            FromDay(DateTime.Today);

        /// <summary>
        /// Creates and returns a new time span that represents the specified day.
        /// </summary>
        /// <param name="day">A particular day.</param>
        /// <returns>A time span representing the specified <paramref name="day"/>.</returns>        
        public static DateTimeSpan FromDay(DateTime day) =>
            FromDay(new DateTimeOffset(day));

        /// <summary>
        /// Creates and returns a new time span that represents the specified day.
        /// </summary>
        /// <param name="day">A particular day.</param>
        /// <returns>A time span representing the specified <paramref name="day"/>.</returns>    
        public static DateTimeSpan FromDay(DateTimeOffset day) =>
            FromDay(day.Year, day.Month, day.Day, day.Offset);

        /// <summary>
        /// Creates and returns a new time span that represents the specified day.
        /// </summary>
        /// <param name="year">A year.</param>
        /// <param name="month">A month.</param>
        /// <param name="day">A day of the month.</param>
        /// <param name="offset">Offset representing the desired time zone.</param>
        /// <returns>A time span that represents the specified day.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="year"/>, <paramref name="month"/> and/or <paramref name="day"/> do not specify a valid day.
        /// </exception>
        public static DateTimeSpan FromDay(int year, int month, int day, TimeSpan? offset = null) =>
            FromDayStart(new DateTimeOffset(year, month, day, 0, 0, 0, ValueOrDefault(offset)));

        private static DateTimeSpan FromDayStart(DateTimeOffset start) =>
            new DateTimeSpan(start, start.AddDays(1));

        private static TimeSpan ValueOrDefault(TimeSpan? offset) =>
            offset ?? TimeSpan.Zero;

        #endregion
    }
}

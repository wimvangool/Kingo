using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class DateTimeSpanTest
    {
        #region [====== Constructor() ======]

        [TestMethod]
        public void DefaultConstructor_ReturnsExpectedInstance()
        {
            var span = new DateTimeSpan();

            Assert.AreEqual(TimeSpan.Zero, span.Start.Offset);
            Assert.AreEqual(TimeSpan.Zero, span.End.Offset);
            Assert.AreEqual(TimeSpan.Zero, span.Duration);
        }

        #endregion

        #region [====== Constructor(DateTimeOffset, TimeSpan) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfDurationIsNegative()
        {
            new DateTimeSpan(DateTime.Today, TimeSpan.FromTicks(-1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfDurationExceedsDateTimeOffsetMaxValue()
        {
            new DateTimeSpan(DateTimeOffset.MaxValue, TimeSpan.FromTicks(1));
        }        

        [TestMethod]
        public void Constructor_ReturnsExpectedInstance_IfDurationIsNonZero()
        {
            var today = new DateTimeOffset(DateTime.Today);
            var duration = TimeSpan.FromDays(1);
            var span = new DateTimeSpan(today, duration);

            Assert.AreEqual(today, span.Start);
            Assert.AreEqual(today.Add(duration), span.End);
            Assert.IsTrue(span.EndInclusive < span.End);
            Assert.AreEqual(duration, span.Duration);
        }

        #endregion

        #region [====== Constructor(DateTimeOffset, DateTimeOffset) ======]        

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfStartIsGreaterThanEnd_And_Both_AreUtcTime()
        {
            CreateTimeSpan(11, TimeSpan.Zero, 10, TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfStartIsGreaterThanEnd_And_Both_AreLocalTime()
        {
            CreateTimeSpan(11, Offset(1), 10, Offset(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfStartIsGreaterThanEnd_And_StartIsLocalTime_And_EndIsUtcTime_And_OffsetIsPositive()
        {
            CreateTimeSpan(12, Offset(1), 10, TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfStartIsGreaterThanEnd_And_StartIsLocalTime_And_EndIsUtcTime_And_OffsetIsNegative()
        {
            CreateTimeSpan(10, Offset(-1), 10, TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfStartIsGreaterThanEnd_And_StartIsUtcTime_And_EndIsLocalTime_And_OffsetIsPositive()
        {
            CreateTimeSpan(10, TimeSpan.Zero, 10, Offset(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfStartIsGreaterThanEnd_And_StartIsUtcTime_And_EndIsLocalTime_And_OffsetIsNegative()
        {
            CreateTimeSpan(12, TimeSpan.Zero, 10, Offset(-1));
        }

        [TestMethod]
        public void Constructor_ReturnsExpectedInstance_IfValidSpanIsSpecified()
        {            
            AssertDuration(TimeSpan.FromHours(1), 0, TimeSpan.Zero, 1, TimeSpan.Zero);
            AssertDuration(TimeSpan.FromHours(1), 1, Offset(1), 2, Offset(1));
            AssertDuration(TimeSpan.FromHours(1), 0, Offset(-1), 1, Offset(-1));
        }

        #endregion

        #region [====== Contains ======]

        [TestMethod]
        public void Contains_ReturnsFalse_IfSpanIsDefaultSpan_And_ValueIsNotDateTimeMinValue()
        {
            Assert.IsFalse(new DateTimeSpan().Contains(DateTime.Today));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfSpanIsDefaultSpan_And_ValueIsDateTimeMinValue()
        {
            Assert.IsTrue(new DateTimeSpan().Contains(DateTimeOffset.MinValue));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IfSpanIsNonDefaultSpan_And_ValueIsLessThanStart()
        {
            var start = new DateTimeOffset(DateTime.Today);
            var end = start.AddDays(3);
            
            Assert.IsFalse(new DateTimeSpan(start, end).Contains(start.Subtract(TimeSpan.FromTicks(1))));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IfSpanIsNonDefaultSpan_And_ValueIsGreaterThanEnd()
        {
            var start = new DateTimeOffset(DateTime.Today);
            var end = start.AddDays(3);

            Assert.IsFalse(new DateTimeSpan(start, end).Contains(end.Add(TimeSpan.FromTicks(1))));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfSpanIsNonDefaultSpan_And_ValueIsEqualToStart()
        {
            var start = new DateTimeOffset(DateTime.Today);
            var end = start.AddDays(3);

            Assert.IsTrue(new DateTimeSpan(start, end).Contains(start));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IfSpanIsNonDefaultSpan_And_ValueIsEqualToEnd()
        {
            var start = new DateTimeOffset(DateTime.Today);
            var end = start.AddDays(3);

            Assert.IsFalse(new DateTimeSpan(start, end).Contains(end));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfSpanIsNonDefaultSpan_And_ValueIsBetweenStartAndEnd()
        {
            var start = new DateTimeOffset(DateTime.Today);
            var end = start.AddDays(3);

            Assert.IsTrue(new DateTimeSpan(start, end).Contains(start.AddDays(1.5)));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfSpanEndIsDateTimeOffsetMaxValue_And_ValueIsDateTimeOffsetMaxValue()
        {
            Assert.IsTrue(DateTimeSpan.MaxValue.Contains(DateTimeOffset.MinValue));
            Assert.IsTrue(DateTimeSpan.MaxValue.Contains(DateTimeOffset.MaxValue));
        }

        #endregion

        #region [====== Enumerate ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Enumerate_Throws_IfStepSizeIsZero()
        {
            DateTimeSpan.MaxValue.Enumerate(TimeSpan.Zero);
        }

        [TestMethod]
        public void Enumerate_ReturnsExpectedPointsInTime_IfStepSizeIsGreaterThanZero()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var pointsInTime = span.Enumerate(TimeSpan.FromHours(1)).ToArray();

            Assert.AreEqual(3, pointsInTime.Length);
            Assert.AreEqual(new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero), pointsInTime[0]);
            Assert.AreEqual(new DateTimeOffset(2018, 1, 1, 1, 0, 0, TimeSpan.Zero), pointsInTime[1]);
            Assert.AreEqual(new DateTimeOffset(2018, 1, 1, 2, 0, 0, TimeSpan.Zero), pointsInTime[2]);
        }

        [TestMethod]
        public void Enumerate_ReturnsExpectedPointsInTime_IfStepSizeIsLessThanZero()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var pointsInTime = span.Enumerate(TimeSpan.FromHours(-1)).ToArray();

            Assert.AreEqual(3, pointsInTime.Length);
            Assert.AreEqual(new DateTimeOffset(2018, 1, 1, 2, 30, 0, TimeSpan.Zero), pointsInTime[0]);
            Assert.AreEqual(new DateTimeOffset(2018, 1, 1, 1, 30, 0, TimeSpan.Zero), pointsInTime[1]);
            Assert.AreEqual(new DateTimeOffset(2018, 1, 1, 0, 30, 0, TimeSpan.Zero), pointsInTime[2]);
        }

        #endregion

        #region [====== Split ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Split_Throws_IfDurationIsZero()
        {
            DateTimeSpan.MaxValue.Split(TimeSpan.Zero);
        }

        // [====== Duration > TimeSpan.Zero ======]

        [TestMethod]
        public void Split_ReturnsEmptyCollection_IfDurationIsGreaterThanZero_And_IncludeRemainderIsFalse_And_RemainderIsNotPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(3)).ToArray();

            Assert.AreEqual(0, spans.Length);
        }

        [TestMethod]
        public void Split_ReturnsSelf_IfDurationIsGreaterThanZero_And_IncludeRemainderIsTrue_And_RemainderIsSelf()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(3), true).ToArray();

            Assert.AreEqual(1, spans.Length);
            Assert.AreEqual(span, spans[0]);
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsGreaterThanZero_And_IncludeRemainderIsFalse_And_RemainderIsNotPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromMinutes(30)).ToArray();
            var a = span.Start;
            var b = new DateTimeOffset(2018, 1, 1, 0, 30, 0, TimeSpan.Zero);
            var c = new DateTimeOffset(2018, 1, 1, 1, 0, 0, TimeSpan.Zero);
            var d = new DateTimeOffset(2018, 1, 1, 1, 30, 0, TimeSpan.Zero);
            var e = new DateTimeOffset(2018, 1, 1, 2, 0, 0, TimeSpan.Zero);
            var f = span.End;

            Assert.AreEqual(5, spans.Length);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[0]);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[1]);
            Assert.AreEqual(new DateTimeSpan(c, d), spans[2]);
            Assert.AreEqual(new DateTimeSpan(d, e), spans[3]);
            Assert.AreEqual(new DateTimeSpan(e, f), spans[4]);
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsGreaterThanZero_And_IncludeRemainderIsFalse_And_RemainderIsPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(1)).ToArray();
            var a = span.Start;
            var b = new DateTimeOffset(2018, 1, 1, 1, 0, 0, TimeSpan.Zero);
            var c = new DateTimeOffset(2018, 1, 1, 2, 0, 0, TimeSpan.Zero);            

            Assert.AreEqual(2, spans.Length);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[0]);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[1]);            
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsGreaterThanZero_And_IncludeRemainderIsTrue_And_RemainderIsNotPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromMinutes(30), true).ToArray();
            var a = span.Start;
            var b = new DateTimeOffset(2018, 1, 1, 0, 30, 0, TimeSpan.Zero);
            var c = new DateTimeOffset(2018, 1, 1, 1, 0, 0, TimeSpan.Zero);
            var d = new DateTimeOffset(2018, 1, 1, 1, 30, 0, TimeSpan.Zero);
            var e = new DateTimeOffset(2018, 1, 1, 2, 0, 0, TimeSpan.Zero);
            var f = span.End;

            Assert.AreEqual(5, spans.Length);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[0]);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[1]);
            Assert.AreEqual(new DateTimeSpan(c, d), spans[2]);
            Assert.AreEqual(new DateTimeSpan(d, e), spans[3]);
            Assert.AreEqual(new DateTimeSpan(e, f), spans[4]);
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsGreaterThanZero_And_IncludeRemainderIsTrue_And_RemainderIsPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(1), true).ToArray();
            var a = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var b = new DateTimeOffset(2018, 1, 1, 1, 0, 0, TimeSpan.Zero);
            var c = new DateTimeOffset(2018, 1, 1, 2, 0, 0, TimeSpan.Zero);
            var d = span.End;

            Assert.AreEqual(3, spans.Length);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[0]);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[1]);
            Assert.AreEqual(new DateTimeSpan(c, d), spans[2]);
        }

        // [====== Duration < TimeSpan.Zero ======]

        [TestMethod]
        public void Split_ReturnsEmptyCollection_IfDurationIsLessThanZero_And_IncludeRemainderIsFalse_And_RemainderIsNotPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(-3)).ToArray();

            Assert.AreEqual(0, spans.Length);
        }

        [TestMethod]
        public void Split_ReturnsSelf_IfDurationIsLessThanZero_And_IncludeRemainderIsTrue_And_RemainderIsSelf()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(-3), true).ToArray();

            Assert.AreEqual(1, spans.Length);
            Assert.AreEqual(span, spans[0]);
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsLessThanZero_And_IncludeRemainderIsFalse_And_RemainderIsNotPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromMinutes(-30)).ToArray();
            var f = span.End;
            var e = new DateTimeOffset(2018, 1, 1, 2, 0, 0, TimeSpan.Zero);
            var d = new DateTimeOffset(2018, 1, 1, 1, 30, 0, TimeSpan.Zero);
            var c = new DateTimeOffset(2018, 1, 1, 1, 0, 0, TimeSpan.Zero);
            var b = new DateTimeOffset(2018, 1, 1, 0, 30, 0, TimeSpan.Zero);
            var a = span.Start;

            Assert.AreEqual(5, spans.Length);
            Assert.AreEqual(new DateTimeSpan(e, f), spans[0]);
            Assert.AreEqual(new DateTimeSpan(d, e), spans[1]);
            Assert.AreEqual(new DateTimeSpan(c, d), spans[2]);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[3]);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[4]);
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsLessThanZero_And_IncludeRemainderIsFalse_And_RemainderIsPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(-1)).ToArray();
            var c = span.End;
            var b = new DateTimeOffset(2018, 1, 1, 1, 30, 0, TimeSpan.Zero);
            var a = new DateTimeOffset(2018, 1, 1, 0, 30, 0, TimeSpan.Zero);                        

            Assert.AreEqual(2, spans.Length);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[0]);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[1]);
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsLessThanZero_And_IncludeRemainderIsTrue_And_RemainderIsNotPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromMinutes(-30), true).ToArray();
            var f = span.End;
            var e = new DateTimeOffset(2018, 1, 1, 2, 0, 0, TimeSpan.Zero);
            var d = new DateTimeOffset(2018, 1, 1, 1, 30, 0, TimeSpan.Zero);
            var c = new DateTimeOffset(2018, 1, 1, 1, 0, 0, TimeSpan.Zero);
            var b = new DateTimeOffset(2018, 1, 1, 0, 30, 0, TimeSpan.Zero);
            var a = span.Start;            

            Assert.AreEqual(5, spans.Length);
            Assert.AreEqual(new DateTimeSpan(e, f), spans[0]);
            Assert.AreEqual(new DateTimeSpan(d, e), spans[1]);
            Assert.AreEqual(new DateTimeSpan(c, d), spans[2]);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[3]);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[4]);
        }

        [TestMethod]
        public void Split_ReturnsExpectedSpans_IfDurationIsLessThanZero_And_IncludeRemainderIsTrue_And_RemainderIsPresent()
        {
            var start = new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var duration = TimeSpan.FromMinutes(150);
            var span = new DateTimeSpan(start, duration);

            var spans = span.Split(TimeSpan.FromHours(-1), true).ToArray();
            var d = span.End;
            var c = new DateTimeOffset(2018, 1, 1, 1, 30, 0, TimeSpan.Zero);
            var b = new DateTimeOffset(2018, 1, 1, 0, 30, 0, TimeSpan.Zero);
            var a = span.Start;

            Assert.AreEqual(3, spans.Length);
            Assert.AreEqual(new DateTimeSpan(c, d), spans[0]);
            Assert.AreEqual(new DateTimeSpan(b, c), spans[1]);
            Assert.AreEqual(new DateTimeSpan(a, b), spans[2]);
        }

        #endregion

        #region [====== GetIntersection ======]

        [TestMethod]
        public void GetIntersection_ReturnsNull_IfNoIntersectionExists()
        {
            var left = DateTimeSpan.FromYear(2017);
            var right = DateTimeSpan.FromYear(2019);

            Assert.IsNull(left.GetIntersection(right));
            Assert.IsNull(right.GetIntersection(left));
        }

        [TestMethod]
        public void GetIntersection_ReturnsSpanWithZeroDuration_IfSpansAreAdjacent()
        {
            var left = DateTimeSpan.FromYear(2017);
            var right = DateTimeSpan.FromYear(2018);

            var intersection = AssertIntersection(left, right);

            Assert.AreEqual(left.End, intersection.Start);
            Assert.AreEqual(left.End, intersection.End);
            Assert.AreEqual(TimeSpan.Zero, intersection.Duration);
        }

        [TestMethod]
        public void GetIntersection_ReturnsIntersection_IfSpansAreEqual()
        {
            var today = DateTimeSpan.Today();

            Assert.AreEqual(today, today.GetIntersection(today));
        }

        [TestMethod]
        public void GetIntersection_ReturnsIntersection_IfSpansPartiallyIntersect()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.Shift(TimeSpan.FromDays(1));

            var intersection = AssertIntersection(left, right);

            Assert.AreEqual(TimeSpan.FromDays(30), intersection.Duration);
            Assert.AreEqual(left.Start.AddDays(1), intersection.Start);
            Assert.AreEqual(left.End, intersection.End);
        }

        [TestMethod]
        public void GetIntersection_ReturnsIntersection_IfOneSpanBeginsWithOtherSpan()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.ShiftEnd(TimeSpan.FromDays(1));

            var intersection = AssertIntersection(left, right);

            Assert.AreEqual(left, intersection);
        }

        [TestMethod]
        public void GetIntersection_ReturnsIntersection_IfOneSpanEndsWithOtherSpan()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.ShiftStart(TimeSpan.FromDays(1));

            var intersection = AssertIntersection(left, right);

            Assert.AreEqual(right, intersection);
        }

        [TestMethod]
        public void GetIntersection_ReturnsIntersection_IfOneSpanFullyContainsOtherSpan()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.ShiftStart(TimeSpan.FromDays(1)).ShiftEnd(TimeSpan.FromDays(-1));

            var intersection = AssertIntersection(left, right);

            Assert.AreEqual(right, intersection);
        }

        private static DateTimeSpan AssertIntersection(DateTimeSpan left, DateTimeSpan right)
        {            
            Assert.IsTrue(left.TryGetIntersection(right, out var intersection));
            Assert.AreEqual(intersection, right.GetIntersection(left));

            return intersection;
        }

        #endregion

        #region [====== GetDifference ======]

        [TestMethod]
        public void GetDifference_ReturnsNoDifference_IfSpansAreEqual()
        {
            var today = DateTimeSpan.Today();

            AssertDifference(today, today);
        }

        [TestMethod]
        public void GetDifference_ReturnsBothSpans_IfSpansDoNotOverlap()
        {
            var left = DateTimeSpan.FromYear(2017);
            var right = DateTimeSpan.FromYear(2019);

            AssertDifference(left, right, left, right);
        }

        [TestMethod]
        public void GetDifference_ReturnsBothSpans_IfSpansAreAdjacent()
        {
            var left = DateTimeSpan.FromYear(2017);
            var right = DateTimeSpan.FromYear(2018);

            AssertDifference(left, right, left, right);
        }

        [TestMethod]
        public void GetDifference_ReturnsExpectedDifference_IfSpansPartiallyOverlap()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.Shift(TimeSpan.FromDays(1));

            var differenceLeft = new DateTimeSpan(left.Start, right.Start);
            var differenceRight = new DateTimeSpan(left.End, right.End);

            AssertDifference(left, right, differenceLeft, differenceRight);
        }

        [TestMethod]
        public void GetDifference_ReturnsExpectedDifference_IfOneSpanBeginsWithOtherSpan()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.ShiftEnd(TimeSpan.FromDays(1));

            var differenceRight = new DateTimeSpan(left.End, right.End);

            AssertDifference(left, right, differenceRight);
        }

        [TestMethod]
        public void GetDifference_ReturnsExpectedDifference_IfOneSpanEndsWithOtherSpan()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.ShiftStart(TimeSpan.FromDays(1));

            var differenceLeft = new DateTimeSpan(left.Start, right.Start);

            AssertDifference(left, right, differenceLeft);
        }

        [TestMethod]
        public void GetDifference_ReturnsExpectedDifference_IfOneSpanFullyContainsOtherSpan()
        {
            var left = DateTimeSpan.FromMonth(2018, 1);
            var right = left.ShiftStart(TimeSpan.FromDays(1)).ShiftEnd(TimeSpan.FromDays(-1));

            var differenceLeft = new DateTimeSpan(left.Start, right.Start);
            var differenceRight = new DateTimeSpan(right.End, left.End);

            AssertDifference(left, right, differenceLeft, differenceRight);
        }

        private static void AssertDifference(DateTimeSpan left, DateTimeSpan right, params DateTimeSpan[] expectedDifference)
        {
            var difference = left.GetDifference(right).ToArray();
            
            AssertAreEqual(expectedDifference, difference);
            AssertAreEqual(difference, right.GetDifference(left));
        }

        private static void AssertAreEqual(IEnumerable<DateTimeSpan> expected, IEnumerable<DateTimeSpan> actual) =>
            AssertAreEqual(new Queue<DateTimeSpan>(expected), new LinkedList<DateTimeSpan>(actual));

        private static void AssertAreEqual(Queue<DateTimeSpan> expected, LinkedList<DateTimeSpan> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            while (expected.Count > 0)
            {
                Assert.IsTrue(actual.Remove(expected.Dequeue()));
            }
        }        

        #endregion

        private static void AssertDuration(TimeSpan expectedDuration, int startHour, TimeSpan startOffset, int endHour, TimeSpan endOffset) =>
            Assert.AreEqual(expectedDuration, CreateTimeSpan(startHour, startOffset, endHour, endOffset));

        private static DateTimeSpan CreateTimeSpan(int startHour, TimeSpan startOffset, int endHour, TimeSpan endOffset) =>
            new DateTimeSpan(CreateDate(startHour, startOffset), CreateDate(endHour, endOffset));
            
        private static DateTimeOffset CreateDate(int hour, TimeSpan offset) =>
            new DateTimeOffset(1, 1, 1, hour, 0, 0, offset);

        private static TimeSpan Offset(int hours) =>
            TimeSpan.FromHours(hours);
    }
}

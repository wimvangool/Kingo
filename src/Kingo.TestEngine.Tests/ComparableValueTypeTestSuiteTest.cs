using System;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class ComparableValueTypeTestSuiteTest
    {
        #region [====== ComparableStub ======]

        private struct ComparableStub : IEquatable<ComparableStub>, IComparable<ComparableStub>, IComparable
        {
            private readonly int _value;            

            public ComparableStub(int value)
            {
                _value = value;                
            }

            #region [====== Equals & GetHashCode ======]
            
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    Coverage.Add(ComparableStubPaths.EqualsNullCompare);
                    return false;
                }
                if (obj is ComparableStub)
                {                    
                    Coverage.Add(ComparableStubPaths.EqualsObjectOfCorrectType);
                    return Equals((ComparableStub) obj);
                }
                Coverage.Add(ComparableStubPaths.EqualsObjectOfDifferentType);
                return false;
            }
            
            public bool Equals(ComparableStub other)
            {
                Coverage.Add(ComparableStubPaths.EqualsDeepCompare);
                return _value == other._value;
            }
            
            public override int GetHashCode()
            {
                Coverage.Add(ComparableStubPaths.GetHashCode);
                return GetType().GetHashCode();
            }
            
            public static bool operator ==(ComparableStub left, ComparableStub right)
            {
                Coverage.Add(ComparableStubPaths.OperatorEquality);
                return left.Equals(right);
            }
            
            public static bool operator !=(ComparableStub left, ComparableStub right)
            {
                Coverage.Add(ComparableStubPaths.OperatorInequality);
                return !left.Equals(right);
            }

            #endregion

            #region [====== CompareTo ======]

            int IComparable.CompareTo(object obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    Coverage.Add(ComparableStubPaths.CompareToNullCompare);
                    throw new ArgumentNullException(nameof(obj));
                }
                if (obj is ComparableStub)
                {
                    Coverage.Add(ComparableStubPaths.CompareToObjectOfCorrectType);
                    return CompareTo((ComparableStub) obj);
                }
                Coverage.Add(ComparableStubPaths.CompareToObjectOfDifferentType);
                throw Comparable.NewUnexpectedTypeException(typeof(ComparableStub), obj.GetType());
            }
            
            public int CompareTo(ComparableStub other)
            {
                Coverage.Add(ComparableStubPaths.CompareToDeepCompare);
                return _value.CompareTo(other._value);
            }
           
            public static bool operator <(ComparableStub left, ComparableStub right)
            {
                Coverage.Add(ComparableStubPaths.OperatorLessThan);
                return Comparable.IsLessThan(left, right);
            }
            
            public static bool operator <=(ComparableStub left, ComparableStub right)
            {
                Coverage.Add(ComparableStubPaths.OperatorLessThanOrEqualTo);
                return Comparable.IsLessThanOrEqualTo(left, right);
            }
            
            public static bool operator >(ComparableStub left, ComparableStub right)
            {
                Coverage.Add(ComparableStubPaths.OperatorGreaterThan);
                return Comparable.IsGreaterThan(left, right);
            }
            
            public static bool operator >=(ComparableStub left, ComparableStub right)
            {
                Coverage.Add(ComparableStubPaths.OperatorGreaterThanOrEqualTo);
                return Comparable.IsGreaterThanOrEqualTo(left, right);
            }

            #endregion

            private static TestSuiteCoverage<ComparableStubPaths> Coverage
            {
                get { return TestSuiteCoverage<ComparableStubPaths>.Current; }
            }

            public static ContextScope<TestSuiteCoverage<ComparableStubPaths>> StartCoverageMeasurement()
            {
                return TestSuiteCoverage<ComparableStubPaths>.StartCoverageMeasurement(new MSTestEngine());
            }
        }

        [Flags]
        private enum ComparableStubPaths
        {
            EqualsNullCompare = 1,
            EqualsObjectOfDifferentType = 2,
            EqualsObjectOfCorrectType = 4,
            EqualsDeepCompare = 8,
            GetHashCode = 16,

            CompareToNullCompare = 32,
            CompareToObjectOfDifferentType = 64,
            CompareToObjectOfCorrectType = 128,
            CompareToDeepCompare = 256,

            OperatorEquality = 512,
            OperatorInequality = 1024,
            OperatorLessThan = 2048,
            OperatorLessThanOrEqualTo = 4096,
            OperatorGreaterThan = 8192,
            OperatorGreaterThanOrEqualTo = 16384
        }

        #endregion

        private ComparableValueTypeTestSuite<ComparableStub> _testSuite;

        [TestInitialize]
        public void Setup()
        {
            _testSuite = new ComparableValueTypeTestSuite<ComparableStub>(new MSTestEngine());
        }       

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void Execute_Throws_IfImplementationIsNotCorrect()
        {
            using (ComparableStub.StartCoverageMeasurement())
            {                
                var parameters = new ComparableTestParameters<ComparableStub>()
                {
                    Instance = new ComparableStub(0),
                    EqualInstance = new ComparableStub(1),
                    LargerInstance = new ComparableStub(0)
                };

                _testSuite.Execute(parameters);
            }
        }

        [TestMethod]
        public void Execute_CoversAllCodePaths_IfImplementationIsCorrect()
        {
            using (var scope = ComparableStub.StartCoverageMeasurement())
            {                
                var parameters = new ComparableTestParameters<ComparableStub>()
                {
                    Instance = new ComparableStub(0),
                    EqualInstance = new ComparableStub(0),
                    LargerInstance = new ComparableStub(1)
                };

                _testSuite.Execute(parameters);

                scope.Value.AssertAllCovered();
            }
        }
    }
}

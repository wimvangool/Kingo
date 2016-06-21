using System;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class ComparableReferenceTypeTestSuiteTest
    {
        #region [====== ComparableStub ======]

        private sealed class ComparableStub : IEquatable<ComparableStub>, IComparable<ComparableStub>, IComparable
        {
            private readonly int _value;            

            public ComparableStub(int value)
            {
                _value = value;                
            }

            #region [====== Equals & GetHashCode ======]
            
            public override bool Equals(object obj)
            {
                return Equals(obj as ComparableStub);                
            }
            
            public bool Equals(ComparableStub other)
            {
                if (ReferenceEquals(other, null))
                {
                    Coverage.Add(ComparableStubPaths.EqualsNullCompare);
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    Coverage.Add(ComparableStubPaths.EqualsSelfCompare);
                    return true;
                }
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
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(ComparableStubPaths.OperatorEqualityLeftNull);
                    return ReferenceEquals(right, null);
                }
                Coverage.Add(ComparableStubPaths.OperatorEquality);
                return left.Equals(right);
            }
            
            public static bool operator !=(ComparableStub left, ComparableStub right)
            {
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(ComparableStubPaths.OperatorInequalityLeftNull);
                    return !ReferenceEquals(right, null);
                }
                Coverage.Add(ComparableStubPaths.OperatorInequality);
                return !left.Equals(right);
            }

            #endregion

            #region [====== CompareTo ======]

            int IComparable.CompareTo(object obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    Coverage.Add(ComparableStubPaths.CompareToObjectNullCompare);
                    return Comparable.Greater;
                }
                var other = obj as ComparableStub;      
                if (other == null)
                {
                    Coverage.Add(ComparableStubPaths.CompareToObjectOfDifferentType);
                    throw Comparable.NewUnexpectedTypeException(typeof(ComparableStub), obj.GetType());                    
                }
                Coverage.Add(ComparableStubPaths.CompareToObjectOfCorrectType);
                return CompareTo(other);
            }
            
            public int CompareTo(ComparableStub other)
            {
                if (ReferenceEquals(other, null))
                {
                    Coverage.Add(ComparableStubPaths.CompareToNullCompare);
                    return Comparable.Greater;
                }
                if (ReferenceEquals(other, this))
                {
                    Coverage.Add(ComparableStubPaths.CompareToSelfCompare);
                    return Comparable.Equal;
                }
                Coverage.Add(ComparableStubPaths.CompareToDeepCompare);
                return _value.CompareTo(other._value);
            }
           
            public static bool operator <(ComparableStub left, ComparableStub right)
            {
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(ComparableStubPaths.OperatorLessThanLeftNull);
                    return !ReferenceEquals(right, null);
                }
                Coverage.Add(ComparableStubPaths.OperatorLessThan);
                return left.CompareTo(right) < 0;
            }
            
            public static bool operator <=(ComparableStub left, ComparableStub right)
            {
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(ComparableStubPaths.OperatorLessThanOrEqualToLeftNull);
                    return true;
                }
                Coverage.Add(ComparableStubPaths.OperatorLessThanOrEqualTo);
                return left.CompareTo(right) <= 0;
            }
            
            public static bool operator >(ComparableStub left, ComparableStub right)
            {
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(ComparableStubPaths.OperatorGreaterThanLeftNull);
                    return false;
                }
                Coverage.Add(ComparableStubPaths.OperatorGreaterThan);
                return left.CompareTo(right) > 0;
            }
            
            public static bool operator >=(ComparableStub left, ComparableStub right)
            {
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(ComparableStubPaths.OperatorGreaterThanOrEqualToLeftNull);
                    return ReferenceEquals(right, null);
                }
                Coverage.Add(ComparableStubPaths.OperatorGreaterThanOrEqualTo);
                return left.CompareTo(right) >= 0;
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
            EqualsSelfCompare = 2,
            EqualsDeepCompare = 4,
            GetHashCode = 8,

            CompareToObjectNullCompare = 16,            
            CompareToObjectOfDifferentType = 32,
            CompareToObjectOfCorrectType = 64,

            CompareToNullCompare = 128,
            CompareToSelfCompare = 256,
            CompareToDeepCompare = 512,

            OperatorEquality = 1024,
            OperatorEqualityLeftNull = 2048,            
            OperatorInequality = 4096,
            OperatorInequalityLeftNull = 8192,
            OperatorLessThan = 16384,
            OperatorLessThanLeftNull = 32768,
            OperatorLessThanOrEqualTo = 65536,
            OperatorLessThanOrEqualToLeftNull = 131072,
            OperatorGreaterThan = 262144,
            OperatorGreaterThanLeftNull = 524288,
            OperatorGreaterThanOrEqualTo = 1048576,
            OperatorGreaterThanOrEqualToLeftNull = 2097152
        }

        #endregion

        private ComparableReferenceTypeTestSuite<ComparableStub> _testSuite;

        [TestInitialize]
        public void Setup()
        {
            _testSuite = new ComparableReferenceTypeTestSuite<ComparableStub>(new MSTestEngine());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceIsNull()
        {
            _testSuite.Execute(new ComparableTestParameters<ComparableStub>()
            {
                EqualInstance = new ComparableStub(0),
                LargerInstance = new ComparableStub(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfEqualsInstanceIsNull()
        {
            _testSuite.Execute(new ComparableTestParameters<ComparableStub>()
            {
                Instance = new ComparableStub(0),
                LargerInstance = new ComparableStub(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfLargerInstanceIsNull()
        {
            _testSuite.Execute(new ComparableTestParameters<ComparableStub>()
            {
                Instance = new ComparableStub(0),
                EqualInstance = new ComparableStub(0)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceAndEqualInstanceAreSameInstance()
        {
            var instance = new ComparableStub(0);

            _testSuite.Execute(new ComparableTestParameters<ComparableStub>()
            {
                Instance = instance,
                EqualInstance = instance,
                LargerInstance = new ComparableStub(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceAndLargerInstanceAreSameInstance()
        {
            var instance = new ComparableStub(0);

            _testSuite.Execute(new ComparableTestParameters<ComparableStub>()
            {
                Instance = instance,
                EqualInstance = new ComparableStub(0),
                LargerInstance = instance
            });
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

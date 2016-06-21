using System;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class EquatableReferenceTypeTestSuiteTest
    {
        #region [====== EquatableStub ======]

        private sealed class EquatableStub : IEquatable<EquatableStub>
        {
            private readonly int _value;            

            public EquatableStub(int value)
            {
                _value = value;                
            }                        

            public override bool Equals(object obj)
            {
                Coverage.Add(EquatableStubPaths.EqualsObject);
                return Equals(obj as EquatableStub);
            }

            public bool Equals(EquatableStub other)
            {                                
                if (ReferenceEquals(other, null))
                {
                    Coverage.Add(EquatableStubPaths.NullCompare);
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    Coverage.Add(EquatableStubPaths.SelfCompare);
                    return true;
                }
                Coverage.Add(EquatableStubPaths.DeepCompare);
                return _value == other._value;
            }

            public override int GetHashCode()
            {
                Coverage.Add(EquatableStubPaths.GetHashCode);
                return _value;
            }            

            public static bool operator ==(EquatableStub left, EquatableStub right)
            {
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(EquatableStubPaths.EqualityOperatorLeftNull);
                    return ReferenceEquals(right, null);
                }
                Coverage.Add(EquatableStubPaths.EqualityOperator);
                return left.Equals(right);
            }

            public static bool operator !=(EquatableStub left, EquatableStub right)
            {
                if (ReferenceEquals(left, null))
                {
                    Coverage.Add(EquatableStubPaths.InequalityOperatorLeftNull);
                    return !ReferenceEquals(right, null);
                }
                Coverage.Add(EquatableStubPaths.InequalityOperator);
                return !left.Equals(right);
            }

            private static TestSuiteCoverage<EquatableStubPaths> Coverage
            {
                get { return TestSuiteCoverage<EquatableStubPaths>.Current; }
            }

            public static ContextScope<TestSuiteCoverage<EquatableStubPaths>> StartCoverageMeasurement()
            {
                return TestSuiteCoverage<EquatableStubPaths>.StartCoverageMeasurement(new MSTestEngine());
            }
        }

        [Flags]
        private enum EquatableStubPaths
        {
            EqualsObject = 1,
            NullCompare = 2,
            SelfCompare = 4,
            DeepCompare = 8,
            GetHashCode = 16,

            EqualityOperator = 32,
            EqualityOperatorLeftNull = 64,
            InequalityOperator = 128,
            InequalityOperatorLeftNull = 256
        }

        #endregion

        private EquatableReferenceTypeTestSuite<EquatableStub> _testSuite;

        [TestInitialize]
        public void Setup()
        {
            _testSuite = new EquatableReferenceTypeTestSuite<EquatableStub>(new MSTestEngine());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceIsNull()
        {
            _testSuite.Execute(new EquatableTestParameters<EquatableStub>()
            {
                EqualInstance = new EquatableStub(0),
                UnequalInstance = new EquatableStub(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfEqualsInstanceIsNull()
        {
            _testSuite.Execute(new EquatableTestParameters<EquatableStub>()
            {
                Instance = new EquatableStub(0),
                UnequalInstance = new EquatableStub(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfUnequalInstanceIsNull()
        {
            _testSuite.Execute(new EquatableTestParameters<EquatableStub>()
            {
                Instance = new EquatableStub(0),
                EqualInstance = new EquatableStub(0)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceAndEqualInstanceAreSameInstance()
        {
            var instance = new EquatableStub(0);

            _testSuite.Execute(new EquatableTestParameters<EquatableStub>()
            {
                Instance = instance,
                EqualInstance = instance,
                UnequalInstance = new EquatableStub(1)
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceAndUnequalInstanceAreSameInstance()
        {
            var instance = new EquatableStub(0);

            _testSuite.Execute(new EquatableTestParameters<EquatableStub>()
            {
                Instance = instance,
                EqualInstance = new EquatableStub(0),
                UnequalInstance = instance
            });
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void Execute_Throws_IfImplementationIsNotCorrect()
        {
            using (EquatableStub.StartCoverageMeasurement())
            {                
                var parameters = new EquatableTestParameters<EquatableStub>()
                {
                    Instance = new EquatableStub(0),
                    EqualInstance = new EquatableStub(1),
                    UnequalInstance = new EquatableStub(0)
                };

                _testSuite.Execute(parameters);
            }
        }

        [TestMethod]
        public void Execute_CoversAllCodePaths_IfImplementationIsCorrect()
        {
            using (var scope = EquatableStub.StartCoverageMeasurement())
            {                
                var parameters = new EquatableTestParameters<EquatableStub>()
                {
                    Instance = new EquatableStub(0),
                    EqualInstance = new EquatableStub(0),
                    UnequalInstance = new EquatableStub(1)
                };

                _testSuite.Execute(parameters);

                scope.Value.AssertAllCovered();
            }
        }
    }
}

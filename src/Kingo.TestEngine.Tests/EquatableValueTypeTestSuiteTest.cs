using System;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class EquatableValueTypeTestSuiteTest
    {
        #region [====== EquatableStub ======]

        private struct EquatableStub : IEquatable<EquatableStub>
        {
            private readonly int _value;            

            public EquatableStub(int value)
            {
                _value = value;                
            }                        

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    Coverage.Add(EquatableStubPaths.NullCompare);
                    return false;
                }
                if (obj is EquatableStub)
                {
                    Coverage.Add(EquatableStubPaths.EqualsObjectOfCorrectType);
                    return Equals((EquatableStub) obj);
                }          
                Coverage.Add(EquatableStubPaths.EqualsObjectOfDifferentType);      
                return false;
            }

            public bool Equals(EquatableStub other)
            {                                
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
                Coverage.Add(EquatableStubPaths.EqualsOperator);
                return left.Equals(right);
            }

            public static bool operator !=(EquatableStub left, EquatableStub right)
            {
                Coverage.Add(EquatableStubPaths.NotEqualsOperator);
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
            NullCompare = 1,
            EqualsObjectOfDifferentType = 2,
            EqualsObjectOfCorrectType = 4,
            DeepCompare = 8,
            GetHashCode = 16,

            EqualsOperator = 32,
            NotEqualsOperator = 64
        }

        #endregion        

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void Execute_Throws_IfImplementationIsNotCorrect()
        {
            using (EquatableStub.StartCoverageMeasurement())
            {
                var testSuite = new EquatableValueTypeTestSuite<EquatableStub>(new MSTestEngine());
                var parameters = new EquatableTestParameters<EquatableStub>()
                {
                    Instance = new EquatableStub(0),
                    EqualInstance = new EquatableStub(1),
                    UnequalInstance = new EquatableStub(0)
                };

                testSuite.Execute(parameters);
            }
        }

        [TestMethod]
        public void Execute_CoversAllCodePaths_IfImplementationIsCorrect()
        {
            using (var scope = EquatableStub.StartCoverageMeasurement())
            {
                var testSuite = new EquatableValueTypeTestSuite<EquatableStub>(new MSTestEngine());
                var parameters = new EquatableTestParameters<EquatableStub>()
                {
                    Instance = new EquatableStub(0),
                    EqualInstance = new EquatableStub(0),
                    UnequalInstance = new EquatableStub(1)
                };

                testSuite.Execute(parameters);

                scope.Value.AssertAllCovered();
            }
        }
    }
}

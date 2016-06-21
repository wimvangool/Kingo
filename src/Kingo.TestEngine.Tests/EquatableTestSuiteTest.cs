using System;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class EquatableTestSuiteTest
    {
        #region [====== EquatableStub ======]

        private sealed class EquatableStub
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

            private bool Equals(EquatableStub other)
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
            GetHashCode = 16
        }

        #endregion        

        private EquatableTestSuite _testSuite;

        [TestInitialize]
        public void Setup()
        {
            _testSuite = new EquatableTestSuite(new MSTestEngine());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceIsNull()
        {
            _testSuite.Execute(new EquatableTestParameters()
            {
                EqualInstance = new object(),
                UnequalInstance = new object()
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfEqualsInstanceIsNull()
        {
            _testSuite.Execute(new EquatableTestParameters()
            {
                Instance = new object(),
                UnequalInstance = new object()
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfUnequalInstanceIsNull()
        {
            _testSuite.Execute(new EquatableTestParameters()
            {
                Instance = new object(),
                EqualInstance = new object()
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceAndEqualInstanceAreSameInstance()
        {
            var instance = new object();

            _testSuite.Execute(new EquatableTestParameters()
            {
                Instance = instance,
                EqualInstance = instance,
                UnequalInstance = new object()
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Execute_Throws_IfInstanceAndUnequalInstanceAreSameInstance()
        {
            var instance = new object();

            _testSuite.Execute(new EquatableTestParameters()
            {
                Instance = instance,
                EqualInstance = new object(),
                UnequalInstance = instance
            });
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void Execute_Throws_IfImplementationIsNotCorrect()
        {
            using (EquatableStub.StartCoverageMeasurement())
            {                
                var parameters = new EquatableTestParameters()
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
                var parameters = new EquatableTestParameters()
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

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace YellowFlare.MessageProcessing.TestEngine
{
    [TestClass]
    public sealed class OperandTest
    {
        #region [====== Nested Types ======]

        private sealed class Int64Comparer : IEqualityComparer<long>, IComparer<long>
        {
            public bool Equals(long x, long y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(long obj)
            {
                return obj.GetHashCode();
            }
        
            public int Compare(long x, long y)
            {
 	            return x.CompareTo(y);
            }
        }

        private sealed class StringComparer : IEqualityComparer<string>, IComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x == y;
            }

            public int GetHashCode(string obj)
            {
                return obj == null ? 0 : obj.GetHashCode();
            }
        
            public int Compare(string x, string y)
            {
 	            if (x == null)
 	            {
 	                return y == null ? 0 : -1;
 	            }
                return x.CompareTo(y);
            }
        }

        #endregion

        private Mock<IScenario> _scenarioMock;

        private IScenario Scenario
        {
            get { return _scenarioMock.Object; }
        }

        [TestInitialize]
        public void Setup()
        {
            _scenarioMock = new Mock<IScenario>(MockBehavior.Strict);            
        }

        [TestCleanup]
        public void Teardown()
        {
            _scenarioMock.VerifyAll();    
        }

        private void ExpectFailure()
        {
            _scenarioMock.Setup(framework => framework.Fail(It.IsAny<string>(), It.IsAny<object[]>()));
        }

        #region [====== Basic Tests ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfScenarioIsNull()
        {
            new Operand<object>(null, new object());
        }

        [TestMethod]
        public void Value_AlwaysReturnsExpression()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            Assert.AreEqual(value, operand.Value);
        }

        #endregion

        #region [====== IsNull() ======]

        [TestMethod]        
        public void IsNull_Fails_IfExpressionIsValueType()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsNull();
        }

        [TestMethod]        
        public void IsNull_Fails_IfExpressionIsReferenceTypeAndNotNull()
        {
            ExpectFailure();

            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsNull();
        }

        [TestMethod]
        public void IsNull_DoesNotFail_IfExpressionIsReferenceTypeAndNull()
        {
            var operand = new Operand<object>(Scenario, null);

            operand.IsNull();
        }

        #endregion

        #region [====== IsNotNull() ======]

        [TestMethod]        
        public void IsNotNull_DoesNotFail_IfExpressionIsValueType()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsNotNull();
        }

        [TestMethod]        
        public void IsNotNull_DoesNotFail_IfExpressionIsReferenceTypeAndNotNull()
        {
            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotNull();
        }

        [TestMethod]        
        public void IsNotNull_Fails_IfExpressionIsReferenceTypeAndNull()
        {
            ExpectFailure();

            var operand = new Operand<object>(Scenario, null);

            operand.IsNotNull();
        }

        #endregion

        #region [====== IsA(Type) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsA_Throws_IfTypeIsNull()
        {
            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsA(null);
        }

        [TestMethod]        
        public void IsA_Fails_IfExpressionIsValueTypeAndNotOfCompatibleType()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            operand.IsA(typeof(int));
        }

        [TestMethod]
        public void IsA_DoesNotFail_IfExpressionIsValueTypeAndOfCompatibleType()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            operand.IsA(typeof(long));
        }

        [TestMethod]        
        public void IsA_Fails_IfExpressionIsReferenceTypeAndNotOfCompatibleType()
        {
            ExpectFailure();

            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsA(typeof(string));
        }     
   
        [TestMethod]
        public void IsA_DoesNotFail_IfExpressionIsReferenceTypeAndOfCompatibleType()
        {
            const string value = "Some value";
            var operand = new Operand<object>(Scenario, value);

            operand.IsA(typeof(string));
        }

        [TestMethod]        
        public void IsA_Fails_IfExpressionIsNullAndNotOfCompatibleType()
        {
            ExpectFailure();

            var operand = new Operand<object>(Scenario, null);

            operand.IsA(typeof(int));
        }

        [TestMethod]
        public void IsA_DoesNotFail_IfExpressionIsNullAndOfCompatibleType()
        {
            var operand = new Operand<string>(Scenario, null);

            operand.IsA(typeof(string));
        }

        #endregion

        #region [====== IsA<TExpected> ======]        

        [TestMethod]        
        public void IsA_ExpectedType_Fails_IfExpressionIsValueTypeAndNotOfCompatibleType()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            Assert.IsNotNull(operand.IsA<int>());
        }

        [TestMethod]
        public void IsA_ExpectedType_DoesNotFail_IfExpressionIsValueTypeAndOfCompatibleType()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            Assert.IsNotNull(operand.IsA<long>());
        }

        [TestMethod]       
        public void IsA_ExpectedType_Fails_IfExpressionIsReferenceTypeAndNotOfCompatibleType()
        {
            ExpectFailure();

            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            Assert.IsNotNull(operand.IsA<string>());
        }

        [TestMethod]
        public void IsA_ExpectedType_DoesNotFail_IfExpressionIsReferenceTypeAndOfCompatibleType()
        {
            const string value = "Some value";
            var operand = new Operand<object>(Scenario, value);

            Assert.IsNotNull(operand.IsA<string>());
        }

        [TestMethod]        
        public void IsA_ExpectedType_Fails_IfExpressionIsNullAndNotOfCompatibleType()
        {
            ExpectFailure();

            var operand = new Operand<object>(Scenario, null);

            Assert.IsNotNull(operand.IsA<int>());
        }

        [TestMethod]
        public void IsA_ExpectedType_DoesNotFail_IfExpressionIsNullAndOfCompatibleType()
        {
            var operand = new Operand<string>(Scenario, null);

            Assert.IsNotNull(operand.IsA<string>());
        }

        #endregion

        #region [====== IsNotA(Type) ======]

        [TestMethod]        
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsNotA_Throws_IfTypeIsNull()
        {            
            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA(null);
        }

        [TestMethod]        
        public void IsNotA_DoesNotFail_IfExpressionIsValueTypeAndNotOfCompatibleType()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA(typeof(int));
        }

        [TestMethod]        
        public void IsNotA_Fails_IfExpressionIsValueTypeAndOfCompatibleType()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA(typeof(long));
        }

        [TestMethod]        
        public void IsNotA_DoesNotFail_IfExpressionIsReferenceTypeAndNotOfCompatibleType()
        {
            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA(typeof(string));
        }

        [TestMethod]        
        public void IsNotA_Fails_IfExpressionIsReferenceTypeAndOfCompatibleType()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA(typeof(string));
        }

        [TestMethod]        
        public void IsNotA_DoesNotFail_IfExpressionIsNullAndNotOfCompatibleType()
        {
            var operand = new Operand<object>(Scenario, null);

            operand.IsNotA(typeof(int));
        }

        [TestMethod]        
        public void IsNotA_Fails_IfExpressionIsNullAndOfCompatibleType()
        {
            ExpectFailure();

            var operand = new Operand<string>(Scenario, null);

            operand.IsNotA(typeof(string));
        }

        #endregion

        #region [====== IsNotA<TUnexpected> ======]

        [TestMethod]        
        public void IsNotA_UnexpectedType_DoesNotFail_IfExpressionIsValueTypeAndNotOfCompatibleType()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA<int>();
        }

        [TestMethod]        
        public void IsNotA_UnexpectedType_Fails_IfExpressionIsValueTypeAndOfCompatibleType()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA<long>();
        }

        [TestMethod]        
        public void IsNotA_UnexpectedType_DoesNotFail_IfExpressionIsReferenceTypeAndNotOfCompatibleType()
        {
            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA<string>();
        }

        [TestMethod]        
        public void IsNotA_UnexpectedType_Fails_IfExpressionIsReferenceTypeAndOfCompatibleType()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotA<string>();
        }

        [TestMethod]        
        public void IsNotA_UnexpectedType_DoesNotFail_IfExpressionIsNullAndNotOfCompatibleType()
        {
            var operand = new Operand<object>(Scenario, null);

            operand.IsNotA<int>();
        }

        [TestMethod]        
        public void IsNotA_UnexpectedType_Fails_IfExpressionIsNullAndOfCompatibleType()
        {
            ExpectFailure();

            var operand = new Operand<string>(Scenario, null);

            operand.IsNotA<string>();
        }

        #endregion

        #region [====== IsTheSameInstanceAs() ======]

        [TestMethod]        
        public void IsTheSameInstanceAs_Fails_IfExpressionIsValueType()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsSameInstanceAs(value);
        }

        [TestMethod]        
        public void IsTheSameInstanceAs_Fails_IfExpressionIsReferenceTypeAndNotTheSameInstance()
        {
            ExpectFailure();

            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsSameInstanceAs(new object());
        }

        [TestMethod]      
        public void IsTheSameInstanceAs_DoesNotFail_IfExpressionIsReferenceTypeAndTheSameInstance()
        {
            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsSameInstanceAs(value);
        }

        #endregion

        #region [====== IsNotTheSameInstanceAs() ======]

        [TestMethod]        
        public void IsNotTheSameInstanceAs_DoesNotFail_IfExpressionIsValueType()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsNotSameInstanceAs(value);
        }

        [TestMethod]        
        public void IsNotTheSameInstanceAs_DoesNotFail_IfExpressionIsReferenceTypeAndNotTheSameInstance()
        {
            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotSameInstanceAs(new object());
        }

        [TestMethod]        
        public void IsNotTheSameInstanceAs_Fails_IfExpressionIsReferenceTypeAndTheSameInstance()
        {
            ExpectFailure();

            var value = new object();
            var operand = new Operand<object>(Scenario, value);

            operand.IsNotSameInstanceAs(value);
        }

        #endregion

        #region [====== IsEqualTo(T) ======]

        [TestMethod]        
        public void IsEqualTo_Fails_IfExpressionIsValueTypeAndNotEqual()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsEqualTo(value + 1L);
        }

        [TestMethod]        
        public void IsEqualTo_DoesNotFail_IfExpressionIsValueTypeAndEqual()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsEqualTo(value);
        }

        [TestMethod]        
        public void IsEqualTo_Fails_IfExpressionIsReferenceTypeAndNotEqual()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsEqualTo("Some other value");
        }

        [TestMethod]        
        public void IsEqualTo_DoesNotFail_IfExpressionIsReferenceTypeAndEqual()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsEqualTo(value);
        }

        #endregion

        #region [====== IsEqualTo(T, IEqualityComparer<T>) ======]        

        [TestMethod]        
        public void IsEqualToByComparer_Fails_IfExpressionIsValueTypeAndNotEqual()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);
            
            operand.IsEqualTo(value + 1L, new Int64Comparer());
        }

        [TestMethod]
        public void IsEqualToByComparer_DoesNotFail_IfExpressionIsValueTypeAndEqual()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsEqualTo(value, new Int64Comparer());
        }

        [TestMethod]        
        public void IsEqualToByComparer_Fails_IfExpressionIsReferenceTypeAndNotEqual()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsEqualTo("Some other value", new StringComparer());
        }

        [TestMethod]
        public void IsEqualToByComparer_DoesNotFail_IfExpressionIsReferenceTypeAndEqual()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsEqualTo(value, new StringComparer());
        }

        #endregion

        #region [====== IsNotEqualTo(T) ======]

        [TestMethod]        
        public void IsNotEqualTo_DoesNotFail_IfExpressionIsValueTypeAndNotEqual()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);
           
            operand.IsNotEqualTo(value + 1L);
        }

        [TestMethod]        
        public void IsNotEqualTo_Fails_IfExpressionIsValueTypeAndEqual()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsNotEqualTo(value);
        }

        [TestMethod]        
        public void IsNotEqualTo_DoesNotFail_IfExpressionIsReferenceTypeAndNotEqual()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsNotEqualTo("Some other value");
        }

        [TestMethod]        
        public void IsNotEqualTo_DoesNotFail_IfExpressionIsReferenceTypeAndEqual()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsNotEqualTo(value);
        }

        #endregion

        #region [====== IsNotEqualTo(T, IEqualityComparer<T>) ======]

        [TestMethod]        
        public void IsNotEqualToByComparer_DoesNotFail_IfExpressionIsValueTypeAndNotEqual()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);
            
            operand.IsNotEqualTo(value + 1L, new Int64Comparer());           
        }

        [TestMethod]        
        public void IsNotEqualToByComparer_Fails_IfExpressionIsValueTypeAndEqual()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsNotEqualTo(value, new Int64Comparer());
        }

        [TestMethod]        
        public void IsNotEqualToByComparer_DoesNotFail_IfExpressionIsReferenceTypeAndNotEqual()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsNotEqualTo("Some other value", new StringComparer());
        }

        [TestMethod]        
        public void IsNotEqualToByComparer_Fails_IfExpressionIsReferenceTypeAndEqual()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsNotEqualTo(value, new StringComparer());
        }

        #endregion

        #region [====== IsSmallerThan(T, IComparer<T>) ======]

        [TestMethod]        
        public void IsSmallerThanByComparer_Fails_IfExpressionIsValueTypeAndNotSmaller()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsSmallerThan(value, new Int64Comparer());
        }

        [TestMethod]
        public void IsSmallerThanByComparer_DoesNotFail_IfExpressionIsValueTypeAndSmaller()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsSmallerThan(value + 1L, new Int64Comparer());
        }

        [TestMethod]        
        public void IsSmallerThanByComparer_Fails_IfExpressionIsReferenceTypeAndNotSmaller()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsSmallerThan(value, new StringComparer());
        }

        [TestMethod]
        public void IsSmallerThanByComparer_DoesNotFail_IfExpressionIsReferenceTypeAndSmaller()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsSmallerThan("Some value that is larger", new StringComparer());
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo(T, IComparer<T>) ======]

        [TestMethod]        
        public void IsSmallerThanOrEqualToByComparer_Fails_IfExpressionIsValueTypeAndGreater()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsSmallerThanOrEqualTo(value - 1L, new Int64Comparer());
        }

        [TestMethod]
        public void IsSmallerThanOrEqualToByComparer_DoesNotFail_IfExpressionIsValueTypeAndEqual()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsSmallerThanOrEqualTo(value, new Int64Comparer());
        }

        [TestMethod]        
        public void IsSmallerThanOrEqualToByComparer_Fails_IfExpressionIsReferenceTypeAndGreater()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsSmallerThanOrEqualTo("A smaller value", new StringComparer());
        }

        [TestMethod]
        public void IsSmallerThanOrEqualToByComparer_DoesNotFail_IfExpressionIsReferenceTypeAndEqual()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsSmallerThanOrEqualTo(value, new StringComparer());
        }

        #endregion

        #region [====== IsGreaterThan(T, IComparer<T>) ======]

        [TestMethod]        
        public void IsGreaterThanByComparer_Fails_IfExpressionIsValueTypeAndNotGreater()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsGreaterThan(value, new Int64Comparer());
        }

        [TestMethod]
        public void IsGreaterThanByComparer_DoesNotFail_IfExpressionIsValueTypeAndGreater()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsGreaterThan(value - 1L, new Int64Comparer());
        }

        [TestMethod]        
        public void IsGreaterThanByComparer_Fails_IfExpressionIsReferenceTypeAndNotGreater()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsGreaterThan(value, new StringComparer());
        }

        [TestMethod]
        public void IsGreaterThanByComparer_DoesNotFail_IfExpressionIsReferenceTypeAndGreater()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsGreaterThan("A smaller value", new StringComparer());
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo(T, IComparer<T>) ======]

        [TestMethod]        
        public void IsGreaterThanOrEqualToByComparer_Fails_IfExpressionIsValueTypeAndSmaller()
        {
            ExpectFailure();

            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsGreaterThanOrEqualTo(value + 1L, new Int64Comparer());
        }

        [TestMethod]
        public void IsGreaterThanOrEqualToByComparer_DoesNotFail_IfExpressionIsValueTypeAndEqual()
        {
            var value = DateTime.Now.Ticks;
            var operand = new Operand<long>(Scenario, value);

            operand.IsGreaterThanOrEqualTo(value, new Int64Comparer());
        }

        [TestMethod]        
        public void IsGreaterThanOrEqualToByComparer_Fails_IfExpressionIsReferenceTypeAndSmaller()
        {
            ExpectFailure();

            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsGreaterThanOrEqualTo("Some value that is larger", new StringComparer());
        }

        [TestMethod]
        public void IsGreaterThanOrEqualToByComparer_DoesNotFail_IfExpressionIsReferenceTypeAndEqual()
        {
            const string value = "Some value";
            var operand = new Operand<string>(Scenario, value);

            operand.IsGreaterThanOrEqualTo(value, new StringComparer());
        }

        #endregion
    }
}

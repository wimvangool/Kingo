using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    [TestClass]
    public sealed class DelegateConstraintBuilderTest : ConstraintTest
    {
        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfDisplayFormatWasInferredFromExpression_IsNotNull()
        {
            var constraint = New.Constraint<object>(value => value != null).BuildConstraint();
            var stringRepresentation = constraint.ToString("Member");
            
            Assert.AreEqual("(Member != null)", stringRepresentation);
        }
    }
}

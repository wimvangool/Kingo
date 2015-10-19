using System.Linq;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    internal static class DataErrorInfoExtensions
    {
        internal static void AssertNoErrors(this DataErrorInfo errorInfo)
        {
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(0, errorInfo.Errors.Count);
        }

        internal static void AssertOneError(this DataErrorInfo errorInfo, string errorMessage)
        {
            AssertOneError(errorInfo, errorMessage, "Member");
        }

        internal static void AssertOneError(this DataErrorInfo errorInfo, string errorMessage, string memberName)
        {            
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(1, errorInfo.Errors.Count);
            Assert.IsTrue(errorInfo.Errors.ContainsKey(memberName), string.Format("Expected member name '{0}' but was '{1}'.", memberName, errorInfo.Errors.Keys.Single()));
            Assert.AreEqual(errorMessage, errorInfo.Errors[memberName]);
        }               
    }
}

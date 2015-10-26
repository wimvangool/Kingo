using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging
{
    internal static class MessageErrorInfoExtensions
    {
        internal static void AssertNoErrors(this MessageErrorInfo errorInfo)
        {
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(0, errorInfo.ErrorCount);
        }

        internal static void AssertOneError(this MessageErrorInfo errorInfo, string errorMessage)
        {
            AssertOneError(errorInfo, errorMessage, "Member");
        }

        internal static void AssertOneError(this MessageErrorInfo errorInfo, string errorMessage, string memberName)
        {            
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(1, errorInfo.ErrorCount);
            Assert.IsTrue(errorInfo.MemberErrors.ContainsKey(memberName), string.Format("Expected member name '{0}' but was '{1}'.", memberName, errorInfo.MemberErrors.Keys.Single()));
            Assert.AreEqual(errorMessage, errorInfo.MemberErrors[memberName]);
        }               
    }
}

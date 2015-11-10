using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging
{
    internal static class MessageErrorInfoExtensions
    {
        internal static void AssertNoErrors(this ErrorInfo errorInfo)
        {
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(0, errorInfo.ErrorCount);
        }        

        internal static ErrorInfo AssertErrorCountIs(this ErrorInfo errorInfo, int count)
        {
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(count, errorInfo.ErrorCount);

            return errorInfo;
        }

        internal static ErrorInfo AssertInstanceError(this ErrorInfo errorInfo, string errorMessage)
        {
            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(errorMessage, errorInfo.Error);

            return errorInfo;
        }

        internal static ErrorInfo AssertMemberError(this ErrorInfo errorInfo, string errorMessage)
        {
            return AssertMemberError(errorInfo, errorMessage, "Member");
        }

        internal static ErrorInfo AssertMemberError(this ErrorInfo errorInfo, string errorMessage, params string[] memberNames)
        {            
            Assert.IsNotNull(errorInfo);            

            foreach (var memberName in memberNames)
            {                
                Assert.IsTrue(errorInfo.MemberErrors.ContainsKey(memberName), "No error for member '{0}' was present.", memberName);
                Assert.AreEqual(errorMessage, errorInfo.MemberErrors[memberName]);    
            }
            return errorInfo;
        }               
    }
}

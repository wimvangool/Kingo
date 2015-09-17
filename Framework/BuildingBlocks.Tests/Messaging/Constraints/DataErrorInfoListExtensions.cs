using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal static class DataErrorInfoListExtensions
    {
        internal static void AssertNoErrors(this IReadOnlyList<DataErrorInfo> errorInfoList)
        {
            Assert.IsNotNull(errorInfoList);
            Assert.AreEqual(0, errorInfoList.Count);
        }

        internal static void AssertOneError(this IReadOnlyList<DataErrorInfo> errorInfoList, string errorMessage)
        {
            AssertOneError(errorInfoList, errorMessage, "Member");
        }

        internal static void AssertOneError(this IReadOnlyList<DataErrorInfo> errorInfoList, string errorMessage, string memberName)
        {
            Assert.IsNotNull(errorInfoList);
            Assert.AreEqual(1, errorInfoList.Count);

            var errorInfo = errorInfoList[0];

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(1, errorInfo.Errors.Count);
            Assert.IsTrue(errorInfo.Errors.ContainsKey(memberName), string.Format("Expected member name '{0}' but was '{1}'.", memberName, errorInfo.Errors.Keys.Single()));
            Assert.AreEqual(errorMessage, errorInfo.Errors[memberName]);
        }               
    }
}

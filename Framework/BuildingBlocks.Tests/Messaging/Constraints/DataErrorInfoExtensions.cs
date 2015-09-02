using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal static class DataErrorInfoExtensions
    {
        internal static void AssertNoErrors(this DataErrorInfo errorTree)
        {
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(0, errorTree.Errors.Count);
        }

        internal static void AssertOneError(this DataErrorInfo errorTree, string errorMessage)
        {
            AssertOneError(errorTree, errorMessage, "Member");
        }

        internal static void AssertOneError(this DataErrorInfo errorTree, string errorMessage, string memberName)
        {
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(1, errorTree.Errors.Count);
            Assert.AreEqual(errorMessage, errorTree.Errors[memberName]);
        }        
    }
}

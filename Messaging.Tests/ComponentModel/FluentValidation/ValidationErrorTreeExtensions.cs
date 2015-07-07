using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Syztem.ComponentModel.FluentValidation
{
    internal static class ValidationErrorTreeExtensions
    {
        internal static void AssertNoErrors(this ValidationErrorTree errorTree)
        {
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(0, errorTree.TotalErrorCount);
        }

        internal static void AssertOneError(this ValidationErrorTree errorTree, string errorMessage)
        {
            AssertOneError(errorTree, errorMessage, "Member");
        }

        internal static void AssertOneError(this ValidationErrorTree errorTree, string errorMessage, string memberName)
        {
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.AreEqual(errorMessage, errorTree.Errors[memberName]);
        }        
    }
}

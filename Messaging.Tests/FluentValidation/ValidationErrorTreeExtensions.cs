using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.FluentValidation
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
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.AreEqual(errorMessage, errorTree.Errors["Member"]);
        }
    }
}

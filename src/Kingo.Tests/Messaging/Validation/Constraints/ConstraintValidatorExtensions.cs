using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation.Constraints
{
    internal static class ConstraintValidatorExtensions
    {
        internal static void AssertToString<TMessage>(this ConstraintMessageValidator<TMessage> messageValidator, string value) where TMessage : class
        {
            Assert.IsNotNull(messageValidator);
            Assert.AreEqual(value, messageValidator.ToString());
        }
    }
}

using Kingo.Messaging.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
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

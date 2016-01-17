using Kingo.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    internal static class ConstraintValidatorExtensions
    {
        internal static void AssertToString<TMessage>(this ConstraintValidator<TMessage> validator, string value) where TMessage : class
        {
            Assert.IsNotNull(validator);
            Assert.AreEqual(value, validator.ToString());
        }
    }
}

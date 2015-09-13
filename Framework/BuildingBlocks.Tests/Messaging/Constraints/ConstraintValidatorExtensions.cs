using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal static class ConstraintValidatorExtensions
    {
        internal static void AssertToString<TMessage>(this ConstraintValidator<TMessage> validator, string value)
        {
            Assert.IsNotNull(validator);
            Assert.AreEqual(value, validator.ToString());
        }
    }
}

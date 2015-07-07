﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syztem.ComponentModel;
using Syztem.ComponentModel.FluentValidation;

namespace SummerBreeze.ChessApplication.Players
{
    [TestClass]
    public sealed class InvalidPasswordScenario : UnitTestScenario<RegisterPlayerCommand>
    {
        protected override RegisterPlayerCommand When()
        {
            var username = "Username";
            var password = PickValueFrom(null, string.Empty, "    ", "abc", "asckvbusyfufjshdksdfs");

            return new RegisterPlayerCommand(username, password);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<InvalidMessageException>().And(ContainsExpectedInnerException);
        }

        private static void ContainsExpectedInnerException(IMemberConstraintSet validator, InvalidMessageException exception)
        {
            validator.VerifyThat(() => exception.InnerException).IsInstanceOf<InvalidPasswordException>();
        }
    }
}
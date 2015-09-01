﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceComponents
{
    [TestClass]
    public sealed class StringTemplateTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Throws_IfFormatIsNull()
        {
            StringTemplate.Parse(null);
        }        

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfClosingBracketIsMissing()
        {
            StringTemplate.Parse(@"{member.Name");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfClosingBracketIsMissing_BecauseOfEscapeCharacter()
        {
            StringTemplate.Parse(@"{member.Name}}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfClosingBracketIsMissing_BecauseOfEndOfString()
        {
            StringTemplate.Parse(@"{");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfOpeningBracketIsMissing()
        {
            StringTemplate.Parse(@"member.Name}");
        }        

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfOpeningBracketIsMissing_BecauseOfEscapeCharacter()
        {
            StringTemplate.Parse(@"{{member.Name}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfOpeningBracketIsMissing_BecauseOfEndOfString()
        {
            StringTemplate.Parse(@"}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfIdentifierIsMissing()
        {
            StringTemplate.Parse(@"{}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfIdentifierIsMissing_BeforeDot()
        {
            StringTemplate.Parse(@"{.Name}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfIdentifierIsMissing_AfterDot()
        {
            StringTemplate.Parse(@"{member.}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfIdentifierIsNotAValidIdentifier()
        {
            StringTemplate.Parse(@"{0}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfExpressionIsNotAValidExpression()
        {
            StringTemplate.Parse(@"{member.0}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfVariableContainsOpenBracket()
        {
            StringTemplate.Parse(@"{memb{er.0}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfVariableContainsFormatInfoOnly()
        {
            StringTemplate.Parse("{:dd-MM-yyyy}");
        }        

        [TestMethod]
        public void Parse_ReturnsExpectedErrorMessage_IfFormatIsEmpty()
        {
            var errorMessage = StringTemplate.Parse(string.Empty);
            
            Assert.AreEqual(string.Empty, errorMessage.ToString());            
        }

        [TestMethod]
        public void ToString_ReturnsExpectedErrorMessage_IfFormatContainsLiteralTextOnly()
        {
            var errorMessageText = Guid.NewGuid().ToString("N");
            var errorMessage = StringTemplate.Parse(errorMessageText);
            
            Assert.AreEqual(errorMessageText, errorMessage.ToString());            
        }

        [TestMethod]
        public void Format_Throws_IfIdentifierIsNull()
        {
            var errorMessageText = Guid.NewGuid().ToString("N");
            var errorMessage = StringTemplate.Parse(errorMessageText);

            errorMessage.Format(null, null);
        }

        [TestMethod]
        public void Format_ReturnsEquivalentErrorMessage_IfIdentifierHasNoMatch()
        {
            const string errorMessageText = @"{member.Name}";
            var errorMessage = StringTemplate.Parse(errorMessageText);
            var formattedMessage = errorMessage.Format("constraint", null);

            Assert.AreEqual(errorMessage, formattedMessage);
        }

        [TestMethod]
        public void Format_ReturnsExpectedErrorMessage_IfFormatContainsVariableOnly()
        {
            const string errorMessageText = @"{member.Name}";
            var errorMessage = StringTemplate.Parse(errorMessageText);
            var memberName = Guid.NewGuid().ToString("N");
            
            Assert.AreEqual(errorMessageText, errorMessage.ToString());            
            Assert.AreEqual(memberName, errorMessage.Format("member", new { Name = memberName }));
        }

        [TestMethod]
        public void Format_ReturnsExpectedErrorMessage_IfFormatContainsVariablesWithDigits()
        {
            const string errorMessageText = @"{member123.Name456}";
            var errorMessage = StringTemplate.Parse(errorMessageText);
            var memberName = Guid.NewGuid().ToString("N");

            Assert.AreEqual(errorMessageText, errorMessage.ToString());            
            Assert.AreEqual(memberName, errorMessage.Format("member123", new { Name456 = memberName }));
        }

        [TestMethod]
        public void Format_ReturnsExpectedErrorMessage_IfFormatContainsMixOfLiteralsAndVariables()
        {
            const string errorMessageText = @"{member.X} and {member.Y}";
            var errorMessage = StringTemplate.Parse(errorMessageText);
            var x = Guid.NewGuid().ToString("N");
            var y = Guid.NewGuid().ToString("N");

            Assert.AreEqual(errorMessageText, errorMessage.ToString());            
            Assert.AreEqual(string.Format("{0} and {1}", x, y), errorMessage.Format("member", new { X = x, Y = y }));
        }

        [TestMethod]
        public void Format_ReturnsExpectedErrorMessage_IfVariableContainsFormatInfo()
        {
            const string errorMessageText = "{member.CurrentDate:dd-MM-yyyy}";
            var errorMessage = StringTemplate.Parse(errorMessageText);
            var currentDate = DateTime.Now;

            Assert.AreEqual(errorMessageText, errorMessage.ToString());            
            Assert.AreEqual(currentDate.ToString("dd-MM-yyyy"), errorMessage.Format("member", new { CurrentDate = currentDate }));
        }        

        [TestMethod]
        public void Format_ReturnsExpectedErrorMessage_IfExpressionIsComplex()
        {
            const string errorMessageText = @"{member.Name.Length}";
            var errorMessage = StringTemplate.Parse(errorMessageText);
            var memberName = Guid.NewGuid().ToString("N");

            Assert.AreEqual(errorMessageText, errorMessage.ToString());            
            Assert.AreEqual(memberName.Length.ToString(), errorMessage.Format("member", new { Name = memberName }));
        }

        [TestMethod]
        public void Parse_ReturnsExpectedErrorMessage_IfLiteralTextContainsEscapeCharacters()
        {
            const string errorMessageText = @"{member.Name} and {{something else}}";
            var errorMessage = StringTemplate.Parse(errorMessageText);
            var memberName = Guid.NewGuid().ToString("N");

            Assert.AreEqual(errorMessageText, errorMessage.ToString());            
            Assert.AreEqual(string.Format("{0} and {{something else}}", memberName), errorMessage.Format("member", new { Name = memberName }));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Format_Throws_IfVariableContainsIllegalIdentifier()
        {
            var errorMessage = StringTemplate.Parse(@"{member.Name}");

            errorMessage.Format("member", new object());
        }        
    }
}

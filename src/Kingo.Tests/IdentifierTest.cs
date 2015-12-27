using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class IdentifierTest
    {
        #region [====== Builder ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NewBuilder_Throws_IfCapacityIsNegative()
        {
            Identifier.NewBuilder(-1);
        }

        [TestMethod]
        public void NewBuilder_ReturnsNewBuilder_IfCapacityIsZero()
        {
            var builder = Identifier.NewBuilder(0);

            Assert.IsNotNull(builder);
            Assert.AreEqual(0, builder.Length);
        }

        [TestMethod]
        public void Append_ReturnsFalse_IfCharacterIsIllegalCharacter()
        {
            var builder = Identifier.NewBuilder();

            Assert.IsFalse(builder.Append('!'));
            Assert.IsFalse(builder.Append('@'));
            Assert.IsFalse(builder.Append('$'));
        }

        [TestMethod]
        public void Append_ReturnsFalse_IfFirstCharacterIsDigit()
        {
            var builder = Identifier.NewBuilder();

            Assert.IsFalse(builder.Append('0'));            
        }

        [TestMethod]
        public void Append_ReturnsTrue_IfCharacterIsLetterOrUnderscore()
        {
            var builder = Identifier.NewBuilder();

            Assert.IsTrue(builder.Append('_'));
            Assert.IsTrue(builder.Append('a'));
        }

        [TestMethod]
        public void Append_ReturnsTrue_IfSecondCharacterIsDigit()
        {
            var builder = Identifier.NewBuilder();

            Assert.IsTrue(builder.Append('_'));
            Assert.IsTrue(builder.Append('8'));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildIdentifier_Throws_IfNoCharactersHaveBeenAppended()
        {
            Identifier.NewBuilder().BuildIdentifier();
        }

        [TestMethod]
        public void BuildIdentifier_ReturnsNewIdentifier_IfSomeCharactersHaveBeenAppended()
        {
            const string identifier = "ab";
            var builder = Identifier.NewBuilder();

            Assert.IsTrue(builder.Append(identifier[0]));
            Assert.IsTrue(builder.Append(identifier[1]));

            var identifierA = builder.BuildIdentifier();
            var identifierB = builder.BuildIdentifier();

            Assert.IsNotNull(identifierA);
            Assert.IsNotNull(identifierB);
            Assert.AreNotSame(identifierA, identifierB);
            Assert.AreEqual(identifier, identifierA.ToString());
            Assert.AreEqual(identifier, identifierB.ToString());
        }

        #endregion

        #region [====== Parse ======]

        [TestMethod]
        public void ParseOrNull_ReturnsNull_IfValueIsNull()
        {
            Assert.IsNull(Identifier.ParseOrNull(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Throws_IfValueIsNull()
        {
            Identifier.Parse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfValueIsEmpty()
        {
            Identifier.Parse(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfValueIsWhiteSpaceOnly()
        {
            Identifier.Parse("    ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfValueStartsWithDigit()
        {
            Identifier.Parse("8abc");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfValueContainsIllegalCharacters()
        {
            Identifier.Parse("abcde!@#$");
        }

        [TestMethod]
        public void Parse_ReturnsIdentifier_IfValueStartsWithUnderscore()
        {
            Assert.IsNotNull(Identifier.Parse("_abcd1234"));
        }

        [TestMethod]
        public void Parse_ReturnsIdentifier_IfValueStartsWithLetter()
        {
            Assert.IsNotNull(Identifier.Parse("abcd1234"));
        }

        #endregion

        #region [====== TryParse ======]       

        [TestMethod]        
        public void TryParse_ReturnsFalse_IfValueIsNull()
        {
            Identifier identifier;

            Assert.IsFalse(Identifier.TryParse(null, out identifier));
            Assert.IsNull(identifier);
        }

        [TestMethod]        
        public void TryParse_ReturnsFalse_IfValueIsEmpty()
        {
            Identifier identifier;

            Assert.IsFalse(Identifier.TryParse(string.Empty, out identifier));
            Assert.IsNull(identifier);
        }

        [TestMethod]        
        public void TryParse_ReturnsFalse_IfValueIsWhiteSpaceOnly()
        {
            Identifier identifier;

            Assert.IsFalse(Identifier.TryParse("    ", out identifier));
            Assert.IsNull(identifier);
        }

        [TestMethod]        
        public void TryParse_ReturnsFalse_IfValueStartsWithDigit()
        {
            Identifier identifier;

            Assert.IsFalse(Identifier.TryParse("8abc", out identifier));
            Assert.IsNull(identifier);
        }

        [TestMethod]        
        public void TryParse_ReturnsFalse_IfValueContainsIllegalCharacters()
        {
            Identifier identifier;

            Assert.IsFalse(Identifier.TryParse("abcde!@#$", out identifier));
            Assert.IsNull(identifier);
        }

        [TestMethod]
        public void TryParse_ReturnsTrue_IfValueStartsWithUnderscore()
        {
            Identifier identifier;

            Assert.IsTrue(Identifier.TryParse("_abcd1234", out identifier));
            Assert.IsNotNull(identifier);
        }

        [TestMethod]
        public void TryParse_ReturnsTrue_IfValueStartsWithLetter()
        {
            Identifier identifier;

            Assert.IsTrue(Identifier.TryParse("abcd1234", out identifier));
            Assert.IsNotNull(identifier);
        }

        #endregion
    }
}

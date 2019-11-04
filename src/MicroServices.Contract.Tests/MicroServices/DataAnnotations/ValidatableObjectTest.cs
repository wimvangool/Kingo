using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.DataAnnotations
{
    [TestClass]
    public sealed class ValidatableObjectTest
    {
        private readonly SomeCommand _command;

        public ValidatableObjectTest()
        {
            _command = new SomeCommand()
            {
                PropertyA = Guid.NewGuid().ToString(),
                PropertyB = 1,
                PropertyC = int.MaxValue
            };
        }

        #region [====== ValidationAttributes ======]

        [TestMethod]
        public void IsNotValid_ReturnsFalse_IfRequestIsValid()
        {
            Assert.IsFalse(_command.IsNotValid(out _));
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfSomeRequiredPropertiesAreNotSpecified()
        {
            _command.PropertyA = null;
            
            AssertIsNotValid(_command, 1, out var results);
            AssertErrors(results, nameof(SomeCommand.PropertyA), 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfSomeCustomValidationAttributesReturnErrors()
        {
            _command.PropertyB = 0;

            AssertIsNotValid(_command, 1, out var results);
            AssertErrors(results, nameof(SomeCommand.PropertyB), 1);
        }

        #endregion

        #region [====== MemberRelations ======]

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfCustomValidationLogicReturnsErrors()
        {
            _command.PropertyC = _command.PropertyB;

            AssertIsNotValid(_command, 1, out var results);
            AssertErrors(results, nameof(SomeCommand.PropertyB), 1);
            AssertErrors(results, nameof(SomeCommand.PropertyC), 1);
        }

        #endregion

        #region [====== ChildMembers (Items) ======]

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfChildMemberItemIsNotValid_And_ChildErrorMessageIsNotSet()
        {
            _command.PropertyD = new SomeCommandData();

            AssertIsNotValid(_command, 1, out var results);
            AssertErrors(results, "PropertyD.PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfChildMemberItemIsNotValid_And_ChildErrorMessageIsSetDirectly()
        {
            _command.PropertyE = new SomeCommandData();

            AssertIsNotValid(_command, 2, out var results);
            AssertErrors(results, "PropertyE", 1);
            AssertError(results, "PropertyE", "PropertyE is not valid.");
            AssertErrors(results, "PropertyE.PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfChildMemberItemIsNotValid_And_ChildErrorMessageIsSetIndirectly()
        {
            _command.PropertyF = new SomeCommandData();

            AssertIsNotValid(_command, 2, out var results);
            AssertErrors(results, "PropertyF", 1);
            AssertError(results, "PropertyF", "PropertyF is not valid.");
            AssertErrors(results, "PropertyF.PropertyA", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsNotValid_Throws_IfChildMemberAttributeIsAppliedIncorrectly()
        {
            _command.PropertyG = new SomeCommandData();
            _command.IsNotValid(out _);
        }

        #endregion

        #region [====== ChildMembers (Collections) ======]

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfCollectionItemIsNotValid()
        {
            _command.PropertyE = new SomeCommandData()
            {
                PropertyA = new object(),
                PropertyB = new List<SomeCommandData>()
                {
                    // Ignored.
                    null,

                    // Invalid.
                    new SomeCommandData(),

                    // Valid.
                    new SomeCommandData()
                    {
                        PropertyA = new object()
                    }
                }
            };

            AssertIsNotValid(_command, 3, out var results);
            AssertError(results, "PropertyE", "PropertyE is not valid.");
            AssertError(results, "PropertyE.PropertyB", "PropertyB contains invalid items.");
            AssertErrors(results, "PropertyE.PropertyB[1].PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfNestedCollectionItemIsNotValid()
        {
            _command.PropertyD = new SomeCommandData()
            {
                PropertyA = new object(),
                PropertyB = new List<SomeCommandData>()
                {                                        
                    new SomeCommandData()
                    {
                        PropertyA = new object(),
                        PropertyB = new List<SomeCommandData>()
                        {
                            // Invalid.
                            new SomeCommandData()
                        }
                    }
                }
            };

            AssertIsNotValid(_command, 3, out var results);            
            AssertError(results, "PropertyD.PropertyB", "PropertyB contains invalid items.");
            AssertError(results, "PropertyD.PropertyB[0].PropertyB", "PropertyB contains invalid items.");
            AssertErrors(results, "PropertyD.PropertyB[0].PropertyB[0].PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfCollectionIsNotValid()
        {
            _command.PropertyD = new SomeCommandData()
            {
                PropertyA = new object(),

                // Invalid
                PropertyC = new SomeCommandCollection()
            };

            AssertIsNotValid(_command, 1, out var results);
            AssertErrors(results, "PropertyD.PropertyC.PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfNestedCollectionIsNotValid()
        {
            _command.PropertyD = new SomeCommandData()
            {
                PropertyA = new object(),
                PropertyC = new SomeCommandCollection(new object())
                {                    
                    new SomeCommandCollection(new object()),
                    new SomeCommandCollection(new object())
                    {
                        // Invalid.
                        new SomeCommandCollection()
                    }
                }
            };

            AssertIsNotValid(_command, 1, out var results);            
            AssertErrors(results, "PropertyD.PropertyC[1][0].PropertyA", 1);            
        }

        #endregion

        #region [====== ChildMembers (Dictionaries) ======]

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfDictionaryItemIsNotValid()
        {
            _command.PropertyE = new SomeCommandData()
            {
                PropertyA = new object(),
                PropertyD = new Dictionary<string, SomeCommandData>()
                {
                    // Ignored.
                    { "null", null },

                    // Invalid.
                    { "invalid", new SomeCommandData() },

                    // Valid.
                    { "valid", new SomeCommandData()
                        {
                            PropertyA = new object()
                        }
                    }
                }
            };

            AssertIsNotValid(_command, 3, out var results);
            AssertError(results, "PropertyE", "PropertyE is not valid.");
            AssertError(results, "PropertyE.PropertyD", "PropertyD contains invalid items.");
            AssertErrors(results, "PropertyE.PropertyD[invalid].PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfNestedDictionaryItemIsNotValid()
        {
            _command.PropertyD = new SomeCommandData()
            {
                PropertyA = new object(),
                PropertyB = new List<SomeCommandData>()
                {
                    new SomeCommandData()
                    {
                        PropertyA = new object(),
                        PropertyD = new Dictionary<string, SomeCommandData>()
                        {
                            // Invalid.
                            { "invalid", new SomeCommandData() }
                        }
                    }
                }
            };

            AssertIsNotValid(_command, 3, out var results);
            AssertError(results, "PropertyD.PropertyB", "PropertyB contains invalid items.");
            AssertError(results, "PropertyD.PropertyB[0].PropertyD", "PropertyD contains invalid items.");
            AssertErrors(results, "PropertyD.PropertyB[0].PropertyD[invalid].PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfDictionaryIsNotValid()
        {
            _command.PropertyD = new SomeCommandData()
            {
                PropertyA = new object(),

                // Invalid
                PropertyE = new SomeCommandDictionary()
            };

            AssertIsNotValid(_command, 1, out var results);
            AssertErrors(results, "PropertyD.PropertyE.PropertyA", 1);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfNestedDictionaryIsNotValid()
        {
            _command.PropertyD = new SomeCommandData()
            {
                PropertyA = new object(),
                PropertyE = new SomeCommandDictionary(new object())
                {
                    { "valid1", new SomeCommandDictionary(new object()) },
                    { "valid2", new SomeCommandDictionary(new object())
                        {
                            // Invalid.
                            { "invalid", new SomeCommandDictionary() }
                        }
                    }
                }
            };

            AssertIsNotValid(_command, 1, out var results);
            AssertErrors(results, "PropertyD.PropertyE[valid2][invalid].PropertyA", 1);
        }

        #endregion

        #region [====== Assertions ======]

        private static void AssertIsNotValid(object request, int errorCount, out ICollection<ValidationResult> results)
        {
            Assert.IsTrue(request.IsNotValid(out results));
            Assert.IsNotNull(results);
            Assert.AreEqual(errorCount, results.Count);
        }

        private static void AssertErrors(IEnumerable<ValidationResult> results, string memberName, int errorCount) =>
            Assert.AreEqual(errorCount, results.SelectMany(result => result.MemberNames).Count(name => name == memberName));

        private static void AssertError(IEnumerable<ValidationResult> results, string memberName, string errorMessage) =>
            Assert.IsTrue(results.Any(result => IsError(result, memberName, errorMessage)), $"Expected error for member '{memberName}' not found.");

        private static bool IsError(ValidationResult result, string memberName, string errorMessage) =>
            result.ErrorMessage == errorMessage && result.MemberNames.Any(name => name == memberName);

        #endregion
    }
}

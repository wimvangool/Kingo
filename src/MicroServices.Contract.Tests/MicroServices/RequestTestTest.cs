using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class RequestTestTest : RequestTest<SomeCommand>
    {
        #region [====== AssertThat(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertThat_Throws_IfRequestIsNull()
        {
            AssertThat(null as SomeCommand);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertThat_Throws_IfRequestConfiguratorIsNull()
        {
            AssertThat(null as Action<SomeCommand>);
        }

        #endregion

        #region [====== AssertThat(...).IsValid() ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void IsValid_Throws_IfRequestIsNotValid()
        {
            try
            {
                AssertThat(command =>
                {
                    command.PropertyA = null;

                }).IsValid();
            }
            catch (TestFailedException exception)
            {
                Assert.AreEqual("Instance of type 'SomeCommand' was expected to be valid, but 1 validation error(s) occurred.", exception.Message);
                throw;
            }            
        }

        [TestMethod]        
        public void IsValid_Succeeds_IfRequestIsValid()
        {
            AssertThat(CreateRequest()).IsValid();
        }

        #endregion

        #region [====== AssertThat(...).IsNotValid(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IsNotValid_Throws_IfExpectedNumberOfValidationErrorsIsNegative()
        {
            AssertThat(command =>
            {
                command.PropertyA = null;

            }).IsNotValid(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IsNotValid_Throws_IfExpectedNumberOfValidationErrorsIsZero()
        {
            AssertThat(command =>
            {
                command.PropertyA = null;

            }).IsNotValid(0);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void IsNotValid_Throws_IfExpectedNumberOfValidationErrorsDoesNotMatchActualNumberOfValidationErrors()
        {
            try
            {
                AssertThat(command =>
                {
                    command.PropertyA = null;

                }).IsNotValid(2);
            }
            catch (TestFailedException exception)
            {
                Assert.AreEqual("The number of expected validation errors (2) does not match the actual amount of validation errors (1).", exception.Message);
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void IsNotValid_Throws_IfInstanceIsValid()
        {
            try
            {
                AssertThat(CreateRequest()).IsNotValid(1);
            }
            catch (TestFailedException exception)
            {
                Assert.AreEqual("Instance of type 'SomeCommand' was expected to be invalid, but no validation errors occurred.", exception.Message);
                throw;
            }
        }

        [TestMethod]        
        public void IsNotValid_Succeeds_IfExpectedNumberOfValidationErrorsMatchesActualNumberOfValidationErrors()
        {
            AssertThat(command =>
            {
                command.PropertyA = null;

            }).IsNotValid(1);
        }

        #endregion

        #region [====== AssertThat(...).IsNotValid(...).And(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void And_Throws_IfDelegateIsNull()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndItem_Throws_IfMemberNameIsNull()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                request[null].IgnoreValue();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AndItem_Throws_IfMemberHasNoErrors()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                var memberName = Guid.NewGuid().ToString();

                try
                {
                    request[memberName].IgnoreValue();
                }
                catch (TestFailedException exception)
                {
                    Assert.AreEqual($"Member '{memberName}' does not have any validation-errors.", exception.Message);
                    throw;
                }                
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndHasError_Throws_IfPredicateIsNull()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                request[nameof(SomeCommand.PropertyA)].HasError(null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AndHasError_Throws_IfPredicateIsNotSatisfiedByAnyErrorMessage()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                try
                {
                    request[nameof(SomeCommand.PropertyA)].HasError(errorMessage => false);
                }
                catch (TestFailedException exception)
                {
                    Assert.AreEqual("Member 'PropertyA' does not contain any validation-error that satisfies the specified predicate.", exception.Message);
                    throw;
                }                
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AndHasError_Throws_IfPredicateIsNotSatisfiedByAnyErrorMessage_And_CustomErrorMessageIsProvided()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                const string message = "a";

                try
                {
                    request[nameof(SomeCommand.PropertyA)].HasError(errorMessage => false, message);
                }
                catch (TestFailedException exception)
                {
                    Assert.AreEqual(message, exception.Message);
                    throw;
                }                
            });
        }

        [TestMethod]        
        public void AndHasError_Succeeds_IfPredicateIsSatisfiedByAnyErrorMessage()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                request[nameof(SomeCommand.PropertyA)].HasError(errorMessage => true);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AndHasError_Throws_IfErrorMessageIsNull()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                request[nameof(SomeCommand.PropertyA)].HasError(null as string);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AndHasError_Throws_IfExpectedErrorMessageComparisonIsNotValid()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                request[nameof(SomeCommand.PropertyA)].HasError(Guid.NewGuid().ToString(), (StringComparison) (-1));
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AndHasError_Throws_IfExpectedErrorMessageIsNotFound()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                var expectedErrorMessage = Guid.NewGuid().ToString();

                try
                {
                    request[nameof(SomeCommand.PropertyA)].HasError(expectedErrorMessage);
                }
                catch (TestFailedException exception)
                {
                    Assert.AreEqual($"Member 'PropertyA' does not have expected validation-error '{expectedErrorMessage}'.", exception.Message);
                    throw;
                }                
            });
        }

        [TestMethod]        
        public void AndHasError_Succeeds_IfExpectedErrorMessageIsFound_And_MemberIsSimpleProperty()
        {
            AssertThat(request =>
            {
                request.PropertyA = null;

            }).IsNotValid(1).And(request =>
            {
                request[nameof(SomeCommand.PropertyA)].HasError("The PropertyA field is required.");
            });
        }

        [TestMethod]
        public void AndHasError_Succeeds_IfExpectedErrorMessageIsFound_And_MembersAreRelated()
        {
            AssertThat(request =>
            {
                request.PropertyB = request.PropertyC;

            }).IsNotValid(1).And(request =>
            {
                request[nameof(SomeCommand.PropertyB)].HasError("PropertyC must be greater than PropertyB");
                request[nameof(SomeCommand.PropertyC)].HasError("PropertyC must be greater than PropertyB");
            });
        }

        [TestMethod]
        public void AndHasError_Succeeds_IfExpectedErrorMessageIsFound_And_MembersIsChildMember()
        {
            AssertThat(request =>
            {
                request.PropertyE = new SomeCommandData();

            }).IsNotValid(2).And(request =>
            {
                request["PropertyE"].HasError("PropertyE is not valid.");
                request["PropertyE.PropertyA"].HasError("The PropertyA field is required.");
            });
        }

        #endregion

        protected override SomeCommand CreateRequest()
        {
            return new SomeCommand()
            {
                PropertyA = Guid.NewGuid().ToString(),
                PropertyB = 2,
                PropertyC = 3
            };
        }            
    }
}

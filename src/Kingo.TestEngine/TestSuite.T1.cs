using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kingo.Messaging;
using Kingo.Messaging.Validation;
using Kingo.Resources;

namespace Kingo
{
    /// <summary>
    /// Serves as a base-class for all tests that are predefined for a specific type.
    /// </summary>
    /// <typeparam name="TParameters">Type of the command that is used to run the test(s).</typeparam>
    public abstract class TestSuite<TParameters> where TParameters : class, IDataTransferObject
    {
        #region [====== TestEngine ======]

        /// <summary>
        /// The <see cref="ITestEngine"/> associated with this test.
        /// </summary>
        protected abstract ITestEngine TestEngine
        {
            get;
        }

        #endregion        

        #region [====== Execution ======]

        /// <summary>
        /// Executes all tests N times, where N is the amount of items in <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">A collection of parameter-instances.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parameters"/> or one of its items is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One of the items of <paramref name="parameters"/> is not valid.
        /// </exception>
        /// <exception cref="Exception">
        /// One of the tests failed.
        /// </exception>
        public void Execute(IEnumerable<TParameters> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            foreach (var parameter in parameters)
            {
                Execute(parameter);
            }
        }

        /// <summary>
        /// Executes all tests.
        /// </summary>
        /// <param name="parameters">Command containing all parameters to run the tests.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parameters"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="parameters"/> is not valid.
        /// </exception>
        /// <exception cref="Exception">
        /// One of the tests failed.
        /// </exception>
        public void Execute(TParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            var errorInfo = parameters.Validate();
            if (errorInfo != null && errorInfo.HasErrors)
            {
                throw NewInvalidParametersException(parameters, errorInfo);
            }
            Run(parameters);
        }

        /// <summary>
        /// Runs all tests.
        /// </summary>
        /// <param name="parameters">A validated command containing all parameters to run the tests.</param>
        /// <exception cref="Exception">
        /// One of the tests failed.
        /// </exception>
        protected abstract void Run(TParameters parameters);

        /// <summary>
        /// Creates and returns an exception indicating the specified command is not valid.
        /// </summary>
        /// <param name="parameters">The invalid command.</param>
        /// <param name="errorInfo">Validation errors.</param>
        /// <returns>A new <see cref="ArgumentException" />.</returns>
        protected virtual ArgumentException NewInvalidParametersException(TParameters parameters, ErrorInfo errorInfo)
        {
            var messageFormat = ExceptionMessages.Test_InvalidCommand;
            var message = string.Format(messageFormat, parameters, GetType().Name, errorInfo);
            return new ArgumentException(message, nameof(parameters));
        }

        #endregion

        #region [====== Exceptions ======]

        /// <summary>
        /// Asserts that the specified exception is thrown when <paramref name="action"/> is invoked.
        /// </summary>
        /// <typeparam name="TException">Type of the expected exception.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="arguments">Arguments of the executed test.</param>
        /// <param name="name">Name of the executed test</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        protected void AssertException<TException>(Action action, TestArguments arguments = null, [CallerMemberName] string name = null) where TException : Exception
        {
            AssertIsTrue(Throws<TException>(action), arguments, name);
        }

        private static bool Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action.Invoke();
                return false;
            }
            catch (TException)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region [====== True & False ======]                      

        /// <summary>
        /// Asserts that the specified <paramref name="value"/> is <c>true</c>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="arguments">Arguments of the executed test.</param>
        /// <param name="name">Name of the executed test.</param>
        /// <exception cref="Exception">
        /// <paramref name="value"/> is <c>false</c>.
        /// </exception>
        protected void AssertIsTrue(bool value, TestArguments arguments = null, [CallerMemberName] string name = null)
        {
            AssertIsFalse(!value, arguments, name);
        }

        /// <summary>
        /// Asserts that the specified <paramref name="value"/> is <c>false</c>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="arguments">Arguments of the executed test.</param>
        /// <param name="name">Name of the executed test.</param>
        /// <exception cref="Exception">
        /// <paramref name="value"/> is <c>true</c>.
        /// </exception>
        protected void AssertIsFalse(bool value, TestArguments arguments = null, [CallerMemberName] string name = null)
        {
            if (value)
            {
                FailTest(arguments, name);
            }
        }

        #endregion

        #region [====== AreEqual & AreNotEqual ======]        

        /// <summary>
        /// Asserts that both values are equal.
        /// </summary>        
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <param name="arguments">Arguments of the executed test.</param>
        /// <param name="name">Name of the executed test.</param>
        /// <exception cref="Exception">
        /// <paramref name="left"/> is not equal to <paramref name="right"/>.
        /// </exception>
        protected void AssertAreEqual<TValue>(TValue left, TValue right, TestArguments arguments = null, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<TValue>.Default.Equals(left, right))
            {
                return;
            }
            FailTest(arguments, name);
        }

        /// <summary>
        /// Asserts that both values are not equal.
        /// </summary>        
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <param name="arguments">Arguments of the executed test.</param>
        /// <param name="name">Name of the executed test.</param>
        /// <exception cref="Exception">
        /// <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </exception>
        protected void AssertAreNotEqual<TValue>(TValue left, TValue right, TestArguments arguments = null, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<TValue>.Default.Equals(left, right))
            {
                FailTest(arguments, name);
            }            
        }

        #endregion

        private void FailTest(TestArguments arguments, string name)
        {
            TestEngine.FailTest(Test.CreateTest(name, arguments));
        }
    }
}

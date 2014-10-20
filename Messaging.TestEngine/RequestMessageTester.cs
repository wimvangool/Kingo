using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Globalization;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// This class can be used to assert the behavior of <see cref="IRequestMessage">messages</see>
    /// when their properties are being changed/assigned.
    /// </summary>
    public class RequestMessageTester
    {
        private readonly IUnitTestFramework _unitTestFramework;
        private readonly List<string> _changedProperties;
        private readonly IRequestMessage _message;
        private int _hasChangesChangedCount;
        private int _isValidChangedCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageTester" /> class.
        /// </summary>
        /// <param name="unitTestFramework">The framework that is used to run the tests.</param>
        /// <param name="message">The message under test.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public RequestMessageTester(IUnitTestFramework unitTestFramework, IRequestMessage message)
        {
            if (unitTestFramework == null)
            {
                throw new ArgumentNullException("unitTestFramework");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _unitTestFramework = unitTestFramework;
            _changedProperties = new List<string>();
            _message = message;
            _message.HasChangesChanged += (s, e) => _hasChangesChangedCount++;
            _message.IsValidChanged += (s, e) => _isValidChangedCount++;
            _message.PropertyChanged += (s, e) =>
            {
                var propertyName = e.PropertyName;
                if (propertyName != "HasChanges" && propertyName != "IsValid" && !_changedProperties.Contains(propertyName))
                {
                    _changedProperties.Add(propertyName);
                }
            };
        }

        /// <summary>
        /// Returns the framework that is used to run the tests.
        /// </summary>
        protected IUnitTestFramework UnitTestFramework
        {
            get { return _unitTestFramework; }
        }

        /// <summary>
        /// Returns the message under test.
        /// </summary>
        protected IRequestMessage Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Returns the simple type name of the <see cref="Message" />.
        /// </summary>
        protected string MessageType
        {
            get { return Message.GetType().Name; }
        }

        #region [====== Change Assertion ======]

        /// <summary>
        /// Asserts that the <see cref="IHasChangesIndicator.HasChangesChanged" />-event was raised exactly <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The expected number of times the event was raied.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="times"/> is less than zero.
        /// </exception>
        public void AssertHasChangesChangedWasRaised(int times)
        {            
            if (times < 0)
            {
                throw NewInvalidTimesArgument(times);
            }
            if (times == _hasChangesChangedCount)
            {
                return;
            }
            UnitTestFramework.FailTest(FailureMessages.MessageChecker_HasChangesChanged_TimesNotEqual, MessageType, times, _hasChangesChangedCount);
        }

        /// <summary>
        /// Asserts that the message has been marked as changed.
        /// </summary>
        public void AssertHasChanged()
        {            
            if (Message.HasChanges)
            {
                return;
            }
            UnitTestFramework.FailTest(FailureMessages.MessageChecker_MessageWasChanged, MessageType);
        }

        /// <summary>
        /// Asserts that the message has not been marked as changed.
        /// </summary>
        public void AssertHasNotChanged()
        {            
            if (Message.HasChanges)
            {
                UnitTestFramework.FailTest(FailureMessages.MessageChecker_MessageWasNotChanged, MessageType);
            }            
        }

        /// <summary>
        /// Asserts that the specified <paramref name="propertyName"/> has been changed.
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        public void AssertHasChanged(string propertyName)
        {            
            if (_changedProperties.Contains(propertyName))
            {
                return;
            }
            UnitTestFramework.FailTest(FailureMessages.MessageChecker_PropertyWasNotChanged, MessageType, propertyName);
        }

        /// <summary>
        /// Asserts that the <see cref="INotifyPropertyChanged.PropertyChanged" />-event was raised exactly <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The expected number of times the event was raied.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="times"/> is less than zero.
        /// </exception>
        public void AssertPropertyChangeCountIs(int times)
        {
            if (times < 0)
            {
                throw NewInvalidTimesArgument(times);
            }
            if (times == _changedProperties.Count)
            {
                return;
            }
            UnitTestFramework.FailTest(FailureMessages.MessageChecker_UnexpectedNumberOfPropertiesChanged, MessageType, times, _changedProperties.Count);
        }

        #endregion

        #region [====== Validation Assertion ======]

        /// <summary>
        /// Asserts that the <see cref="IIsValidIndicator.IsValidChanged" />-event was raised exactly <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The expected number of times the event was raied.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="times"/> is less than zero.
        /// </exception>
        public void AssertIsValidChangedWasRaised(int times)
        {
            if (times < 0)
            {
                throw NewInvalidTimesArgument(times);
            }
            if (times == _isValidChangedCount)
            {
                return;
            }
            UnitTestFramework.FailTest(FailureMessages.MessageChecker_IsValidChanged_TimesNotEqual, MessageType, times, _isValidChangedCount);            
        }

        /// <summary>
        /// Asserts that the message is currently valid.
        /// </summary>
        public void AssertIsValid()
        {
            if (!Message.IsValid)
            {
                UnitTestFramework.FailTest(FailureMessages.MessageChecker_MessageNotValid, MessageType);
                return;
            }
            if (!string.IsNullOrEmpty(Message.Error))
            {
                UnitTestFramework.FailTest(FailureMessages.MessageChecker_MessageHasErrors, MessageType);
            }            
        }

        /// <summary>
        /// Asserts that the message is not currently not valid.
        /// </summary>        
        public void AssertIsNotValid()
        {
            AssertIsNotValid(false);
        }

        /// <summary>
        /// Asserts that the message is not currently not valid.
        /// </summary>
        /// <param name="expectErrors">Indicates whether or not any specific errors were set on the message (<see cref="IDataErrorInfo.Error" />).</param>
        public void AssertIsNotValid(bool expectErrors)
        {
            if (Message.IsValid)
            {
                UnitTestFramework.FailTest(FailureMessages.MessageChecker_MessageIsValid, MessageType);
                return;
            }
            if (expectErrors)
            {
                if (string.IsNullOrEmpty(Message.Error))
                {
                    UnitTestFramework.FailTest(FailureMessages.MessageChecker_MessageHasNoErrors, MessageType);
                }
                return;
            }
            if (!string.IsNullOrEmpty(Message.Error))
            {
                UnitTestFramework.FailTest(FailureMessages.MessageChecker_MessageHasErrors, MessageType);
            }            
        }

        /// <summary>
        /// Asserts that the specified <paramref name="propertyName"/> is not valid and that the message contains an error for it.
        /// </summary>
        /// <param name="propertyName">The property to check.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName"/> is <c>null</c>.
        /// </exception>
        public void AssertHasError(string propertyName)
        {            
            if (string.IsNullOrEmpty(Message[propertyName]))
            {
                UnitTestFramework.FailTest(FailureMessages.MessageChecker_PropertyHasNoErrors, MessageType, propertyName);
            }            
        }

        #endregion

        private static Exception NewInvalidTimesArgument(int times)
        {
            var messageFormat = ExceptionMessages.MessageTester_InvalidTimesArgument;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, times);
            return new ArgumentOutOfRangeException("times", message);
        }
    }
}

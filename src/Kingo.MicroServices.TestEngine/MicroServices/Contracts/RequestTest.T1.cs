using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// When implemented by a class, represents a test-class for a specific <see cref="Request" />.
    /// </summary>    
    public abstract class RequestTest<TRequest> where TRequest : class
    {
        #region [====== AssertIsValid ======]

        

        private static Exception NewInstanceNotValidException(object instance, ICollection<ValidationResult> results)
        {
            var messageFormat = ExceptionMessages.DataContractTest_InstanceNotValid;
            var message = string.Format(messageFormat, instance.GetType().FriendlyName(), results.Count);
            return new TestFailedException(message);
        }

        #endregion

        #region [====== AssertIsNotValid ======]

              
        
        private static Exception NewInstanceValidException(object instance)
        {
            var messageFormat = ExceptionMessages.DataContractTest_InstanceNotValid;
            var message = string.Format(messageFormat, instance.GetType().FriendlyName());
            return new TestFailedException(message);
        }

        #endregion
    }
}

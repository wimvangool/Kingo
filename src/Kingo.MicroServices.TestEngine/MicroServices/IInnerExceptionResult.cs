using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a result that can verify the type and details a an inner-exception.
    /// </summary>
    public interface IInnerExceptionResult
    {
        /// <summary>
        /// Asserts that the inner-exception of another exception is of type <typeparamref name="TException"/>.
        /// </summary>        
        /// <param name="assertion">Optional delegate to assert the details of the exception.</param>
        IInnerExceptionResult WithInnerExceptionOfType<TException>(Action<TException> assertion = null) where TException : Exception;
    }
}

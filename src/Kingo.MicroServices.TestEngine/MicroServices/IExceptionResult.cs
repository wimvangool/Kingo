using System;

namespace Kingo.MicroServices
{
    public interface IInnerExceptionResult
    {
        /// <summary>
        /// Asserts that the inner-exception of another exception is of type <typeparamref name="TException"/>.
        /// </summary>        
        /// <param name="assertion">Optional delegate to assert the details of the exception.</param>
        IInnerExceptionResult WithInnerExceptionOfType<TException>(Action<TException> assertion = null) where TException : Exception;
    }
}

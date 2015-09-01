using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Threading
{
    internal static class TaskExtensions
    {        
        internal static void WaitAndHandle<TException>(this Task task) where TException : class
        {
            try
            {
                task.Wait();

                Assert.Fail("Exception of type '{0}' was expected but no exception was thrown.", typeof(TException));
            }
            catch (AggregateException exception)
            {
                exception.Handle(innerException => innerException is TException);
            }
        }        
    }
}

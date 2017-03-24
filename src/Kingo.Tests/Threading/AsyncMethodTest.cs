using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Threading
{
    [TestClass]
    public sealed class AsyncMethodTest
    {
        [TestMethod]
        public void RunSynchronously_BehavesExactlyAsRunTask_IfNoExceptionIsThrown_And_TaskIsBlockWaited()
        {
            var returnValue = Clock.Current.UtcDateAndTime().Millisecond;
            var resultX = Task.Run(() => returnValue).Result;
            var resultY = AsyncMethod.RunSynchronously(() => returnValue).Result;

            Assert.AreEqual(returnValue, resultX);
            Assert.AreEqual(returnValue, resultY);
        }

        [TestMethod]
        public void RunSynchronously_BehavesExactlyAsRunTask_IfExceptionIsThrown_And_TaskIsBlockWaited()
        {
            var exception = new Exception();
            AggregateException exceptionX = null;
            AggregateException exceptionY = null;

            try
            {
                Task.Run(() => { throw exception; }).Wait();
            }
            catch (AggregateException e)
            {
                exceptionX = e;
            }

            try
            {
                AsyncMethod.RunSynchronously(() => { throw exception; }).Wait();
            }
            catch (AggregateException e)
            {
                exceptionY = e;
            }
            Assert.IsNotNull(exceptionX);
            Assert.AreEqual(1, exceptionX.InnerExceptions.Count);
            Assert.AreSame(exception, exceptionX.InnerExceptions[0]);

            Assert.IsNotNull(exceptionY);
            Assert.AreEqual(1, exceptionY.InnerExceptions.Count);
            Assert.AreSame(exception, exceptionY.InnerExceptions[0]);
        }

        [TestMethod]
        public async Task RunSynchronously_BehavesExactlyAsRunTask_IfNoExceptionIsThrown_And_TaskIsAwaited()
        {
            var returnValue = Clock.Current.UtcDateAndTime().Millisecond;
            var resultX = await Task.Run(() => returnValue);
            var resultY = await AsyncMethod.RunSynchronously(() => returnValue);

            Assert.AreEqual(returnValue, resultX);
            Assert.AreEqual(returnValue, resultY);
        }

        [TestMethod]
        public async Task RunSynchronously_BehavesExactlyAsRunTask_IfExceptionIsThrown_And_TaskIsAwaited()
        {
            var exception = new Exception();
            Exception exceptionX = null;
            Exception exceptionY = null;

            try
            {
                await Task.Run(() => { throw exception; });
            }
            catch (Exception e)
            {
                exceptionX = e;
            }

            try
            {
                await AsyncMethod.RunSynchronously(() => { throw exception; });
            }
            catch (Exception e)
            {
                exceptionY = e;
            }            
            Assert.AreSame(exception, exceptionX);            
            Assert.AreSame(exception, exceptionY);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task RunSynchronously_BehavesExactlyAsRunTask_IfMultipleExceptionsAreThrown_And_TaskIsAwaited()
        {
            var exception = new Exception();
            var taskX = Task.Run(() => { throw exception; });
            var taskY = AsyncMethod.RunSynchronously(() => { throw exception; });           

            try
            {
                await Task.WhenAll(taskX, taskY);                
            }
            catch (Exception e)
            {
                Assert.AreSame(exception, e);
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void RunSunchronously_BehavesExactlyAsRunTask_IfTaskIsCanceled()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.Cancel();

                var taskX = Task.Run(() => tokenSource.Token.ThrowIfCancellationRequested(), tokenSource.Token);
                var taskY = AsyncMethod.RunSynchronously(() => tokenSource.Token.ThrowIfCancellationRequested(), tokenSource.Token);

                try
                {
                    Task.WaitAll(taskX, taskY);
                }
                catch (AggregateException exception)
                {
                    Assert.AreEqual(2, exception.InnerExceptions.Count);
                    Assert.IsInstanceOfType(exception.InnerExceptions[0], typeof(OperationCanceledException));
                    Assert.IsInstanceOfType(exception.InnerExceptions[1], typeof(OperationCanceledException));

                    Assert.IsTrue(taskX.IsCanceled);
                    Assert.IsTrue(taskY.IsCanceled);
                    throw;
                }
            }
        }
    }
}

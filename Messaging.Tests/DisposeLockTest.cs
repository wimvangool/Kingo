using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel
{
    [TestClass]
    public sealed class DisposeLockTest
    {
        private DisposeLock _lock;

        [TestInitialize]
        public void Setup()
        {
            _lock = new DisposeLock(new object());
        }        

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfInstanceIsNull()
        {
            new DisposeLock(null);
        }

        private IDisposable Lock
        {
            get { return _lock; }
        }

        #region [====== EnterDispose / ExitDispose ======]

        [TestMethod]
        [ExpectedException(typeof(SynchronizationLockException))]
        public void ExitDispose_Throws_IfStateIsActive_And_EnterDisposeHasNotBeenInvoked()
        {
            _lock.ExitDispose();
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationLockException))]
        public void ExitDispose_Throws_IfStateIsDisposed_And_EnterDisposeHasNotBeenInvoked()
        {
            _lock.EnterDispose();           
            _lock.ExitDispose();

            _lock.ExitDispose();
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationLockException))]
        public void ExitDispose_Throws_IfStateIsDisposing_And_EnterDisposeHasNotBeenInvoked_OnAnotherThread()
        {
            _lock.EnterDispose();            

            try
            {
                Task.WaitAll(Task.Factory.StartNew(() => _lock.ExitDispose()));
            }
            catch (AggregateException exception)
            {
                throw exception.InnerExceptions[0];
            }
            finally
            {
                _lock.ExitDispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationLockException))]
        public void ExitDispose_Throws_IfStateIsDisposed_And_EnterDisposeHasNotBeenInvoked_OnAnotherThread()
        {
            _lock.EnterDispose();
            _lock.ExitDispose();

            try
            {
                Task.WaitAll(Task.Factory.StartNew(() => _lock.ExitDispose()));
            }
            catch (AggregateException exception)
            {                
                throw exception.InnerExceptions[0];
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationLockException))]
        public void ExitDispose_Throws_IfThreadHasNotExittedEnterMethod()
        {
            _lock.EnterDispose();

            _lock.EnterMethod();

            _lock.ExitDispose();
        }

        [TestMethod]
        public void EnterExitDispose_CanBeInvokedWithoutSideEffects_IfStateIsDisposed()
        {
            _lock.EnterDispose();
            _lock.ExitDispose();
        }

        [TestMethod]
        public void EnterExitDispose_CanBeInvokedInANestedFashion()
        {
            _lock.EnterDispose();
                _lock.EnterDispose();
                _lock.ExitDispose();
            _lock.ExitDispose();
        }

        [TestMethod]
        public void EnterExitDispose_CanBeInvokedInANestedFashionManyTimes()
        {
            _lock.EnterDispose();
                _lock.EnterDispose();
                    _lock.EnterDispose();
                    _lock.ExitDispose();
                _lock.ExitDispose();
            _lock.ExitDispose();
        }

        [TestMethod]
        public void EnterExitDispose_CanBeInvokedWithoutSideEffects_OnAnotherThread_IfStateIsActive()
        {
            using (var waitHandle = new AutoResetEvent(false))
            {
                var task = Task.Factory.StartNew(() =>
                {
                    waitHandle.WaitOne();

                    _lock.EnterDispose();

                    waitHandle.Set();
                    waitHandle.WaitOne();

                    _lock.ExitDispose();                    
                });

                _lock.EnterDispose();

                waitHandle.Set();
                waitHandle.WaitOne();

                _lock.ExitDispose();

                waitHandle.Set();                

                Task.WaitAll(task);
            }
        }
        
        [TestMethod]
        public void EnterExitDispose_CanBeInvokedWithoutSideEffects_OnAnotherThread_IfStateIsDisposing()
        {
            _lock.EnterDispose();

            try
            {
                Task.WaitAll(Task.Factory.StartNew(() =>
                {
                    _lock.EnterDispose();
                    _lock.ExitDispose();                    
                }));                
            }
            finally
            {
                _lock.ExitDispose();
            }
        }

        [TestMethod]
        public void EnterExitDispose_CanBeInvokedWithoutSideEffects_OnAnotherThread_IfStateIsDisposed()
        {
            _lock.EnterDispose();
            _lock.ExitDispose();

            Task.WaitAll(Task.Factory.StartNew(() =>
            {
                _lock.EnterDispose();
                _lock.ExitDispose();
            }));
        }

        [TestMethod]
        [ExpectedException(typeof(LockRecursionException))]
        public void EnterDispose_Throws_IfThreadHasFirstInvokedEnterMethod()
        {
            _lock.EnterMethod();
            _lock.EnterDispose();           
        }

        #endregion

        #region [====== EnterMethod / ExitMethod ======]

        [TestMethod]
        public void EnterExitMethod_CanBeInvoked_IfStateIsActive()
        {
            _lock.EnterMethod();
            _lock.ExitMethod();
        }

        [TestMethod]
        public void EnterExitMethod_CanBeInvoked_IfStateIsDisposing()
        {
            _lock.EnterDispose();

            _lock.EnterMethod();
            _lock.ExitMethod();

            _lock.ExitDispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void EnterMethod_Throws_OnAnotherThread_IfStateIsDisposing()
        {
            _lock.EnterDispose();

            try
            {
                Task.WaitAll(new [] { Task.Factory.StartNew(() => _lock.EnterMethod()) }, TimeSpan.FromMilliseconds(500));
            }
            catch (AggregateException exception)
            {
                throw exception.InnerExceptions[0];
            }
            finally
            {
                _lock.ExitDispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void EnterMethod_Throws_OnAnotherThread_IfStateIsDisposed()
        {
            Lock.Dispose();

            try
            {
                Task.WaitAll(new[] { Task.Factory.StartNew(() => _lock.EnterMethod()) }, TimeSpan.FromMilliseconds(500));
            }
            catch (AggregateException exception)
            {
                throw exception.InnerExceptions[0];
            }
        }

        [TestMethod]
        public void EnterMethod_CanBeInvokedWithoutSideEffects_OnAnotherThread_IfStateIsDisposing_And_ThreadHasAlreadyEnteredMethodBefore()
        {
            using (var waitHandle = new AutoResetEvent(false))
            {
                var task = Task.Factory.StartNew(() =>
                {
                    _lock.EnterMethod();

                    // We signal the handle and then wait for _lock.EnterDispose() to be called.
                    // Then, we will simulate a long task, such that we ensure that _lock.EnterDispose()
                    // will be called before we proceed.
                    waitHandle.Set();
                    
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));

                    // Here, the other thread has moved to the lock into the disposing state.
                    // We now simulate entering (another) method by calling _lock.EnterMethod()
                    // again. This should be allowed.
                    _lock.EnterMethod();

                    // Now we simulate finishing our work by exitting both methods.
                    // This should cause the other thread to enter the write-lock and proceed.
                    _lock.ExitMethod();
                    _lock.ExitMethod();
                });

                // We'll wait for _lock.EnterMethod() to be called.
                waitHandle.WaitOne();
                
                // Then we will enter the dispose method, which will block the current thread
                // because the other one still holds a read-lock.
                _lock.EnterDispose();

                // Here, the other thread has finished it's work, and so we can completed Dispose().
                _lock.ExitDispose();

                Task.WaitAll(task);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationLockException))]
        public void ExitMethod_Throws_IfStateIsDisposing_And_EnterMethodWasNotInvokedFirst()
        {
            _lock.EnterDispose();

            try
            {
                Task.WaitAll(Task.Factory.StartNew(() => _lock.ExitMethod()));
            }
            catch (AggregateException exception)
            {
                throw exception.InnerExceptions[0];
            }
            finally
            {
                _lock.ExitDispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationLockException))]
        public void ExitMethod_Throws_IfStateIsDisposed()
        {
            Lock.Dispose();

            _lock.ExitMethod();
        }

        #endregion
    }
}

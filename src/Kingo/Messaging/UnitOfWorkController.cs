using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class UnitOfWorkController : Disposable, IUnitOfWorkController
    {
        #region [====== NullController ======]

        private sealed class NullController : IUnitOfWorkController
        {                                  
            public Task EnlistAsync(IUnitOfWork unitOfWork, object resourceId = null)
            {
                if (unitOfWork == null)
                {
                    throw new ArgumentNullException(nameof(unitOfWork));
                }
                if (unitOfWork.RequiresFlush())
                {
                    return unitOfWork.FlushAsync();
                }
                return Task.CompletedTask;
            }

            public bool RequiresFlush() =>
                false;

            public Task FlushAsync() =>
                throw NewOperationNotSupportedException(nameof(FlushAsync));

            private static Exception NewOperationNotSupportedException(string methodName)
            {
                var messageFormat = ExceptionMessages.NullController_OperationNotSupported;
                var message = string.Format(messageFormat, methodName);
                return new NotSupportedException(message);
            }
        }       

        #endregion 
        
        public static readonly IUnitOfWorkController None = new NullController();       
        
        private readonly IUnitOfWorkController _controller;

        internal UnitOfWorkController()
        {            
            _controller = new UnitOfWorkControllerImplementation();
        }                     

        public Task EnlistAsync(IUnitOfWork unitOfWork, object resourceId = null) =>
            _controller.EnlistAsync(unitOfWork, resourceId);

        public bool RequiresFlush() =>
            _controller.RequiresFlush();
        
        public Task FlushAsync() =>
            _controller.FlushAsync();                       
    }
}

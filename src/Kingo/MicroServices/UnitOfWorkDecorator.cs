using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class UnitOfWorkDecorator : UnitOfWork
    {
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkDecorator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Task EnlistAsync(IUnitOfWorkResourceManager resourceManager) =>
            _unitOfWork.EnlistAsync(resourceManager);

        internal override Task FlushAsync() =>
            Task.CompletedTask;

        internal override UnitOfWork Decorate() =>
            this;
    }
}

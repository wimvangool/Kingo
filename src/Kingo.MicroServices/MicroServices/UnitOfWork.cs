using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        #region [====== DisabledMode ======]

        private sealed class DisabledMode : IUnitOfWork
        {
            public async Task EnlistAsync(IUnitOfWorkResourceManager resourceManager)
            {                
                if (resourceManager.RequiresFlush())
                {
                    await resourceManager.FlushAsync().ConfigureAwait(false);
                }
            }

            public Task FlushAsync() =>
                Task.CompletedTask;
        }

        #endregion

        #region [====== EnabledMode ======]

        private abstract class EnabledMode : IUnitOfWork
        {
            private static readonly object _NullResourceId = new object();
            private Dictionary<object, List<IUnitOfWorkResourceManager>> _resourceGroups;

            protected EnabledMode()
            {
                _resourceGroups = new Dictionary<object, List<IUnitOfWorkResourceManager>>();
            }

            #region [====== EnlistAsync ======]

            public Task EnlistAsync(IUnitOfWorkResourceManager resourceManager) =>
                AsyncMethod.Run(() => Enlist(resourceManager));

            private void Enlist(IUnitOfWorkResourceManager resourceManager)
            {                
                var resourceId = resourceManager.ResourceId ?? _NullResourceId;

                if (_resourceGroups.TryGetValue(resourceId, out var resourceGroup))
                {
                    if (resourceGroup.Contains(resourceManager))
                    {
                        return;
                    }
                    resourceGroup.Add(resourceManager);
                }
                else
                {
                    _resourceGroups.Add(resourceId, new List<IUnitOfWorkResourceManager> { resourceManager });
                }
            }

            #endregion

            #region [====== FlushAsync ======]

            public Task FlushAsync() =>
                FlushAsync(Interlocked.Exchange(ref _resourceGroups, new Dictionary<object, List<IUnitOfWorkResourceManager>>()).Values);

            protected abstract Task FlushAsync(IEnumerable<List<IUnitOfWorkResourceManager>> resourceGroups);                

            #endregion
        }

        #endregion

        #region [====== SingleThreadedMode ======]

        private sealed class SingleThreadedMode : EnabledMode
        {
            protected override async Task FlushAsync(IEnumerable<List<IUnitOfWorkResourceManager>> resourceGroups)
            {
                foreach (var resourceGroup in resourceGroups)
                {
                    await FlushGroupAsync(resourceGroup).ConfigureAwait(false);
                }
            }
        }

        #endregion

        #region [====== MultiThreadedMode ======]

        private sealed class MultiThreadedMode : EnabledMode
        {
            protected override Task FlushAsync(IEnumerable<List<IUnitOfWorkResourceManager>> resourceGroups) =>
                Task.WhenAll(resourceGroups.Select(FlushGroupAsync));
        }

        #endregion

        private readonly IUnitOfWork _unitOfWork;

        private UnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task EnlistAsync(IUnitOfWorkResourceManager resourceManager) =>
            _unitOfWork.EnlistAsync(resourceManager ?? throw new ArgumentNullException(nameof(resourceManager)));

        public Task FlushAsync() =>
            _unitOfWork.FlushAsync();

        private static async Task FlushGroupAsync(IEnumerable<IUnitOfWorkResourceManager> resourceGroup)
        {
            foreach (var resourceManager in resourceGroup.Where(resource => resource.RequiresFlush()))
            {
                await resourceManager.FlushAsync().ConfigureAwait(false);
            }
        }

        public static UnitOfWork InMode(UnitOfWorkMode mode) =>
            new UnitOfWork(CreateUnitOfWorkMode(mode));

        private static IUnitOfWork CreateUnitOfWorkMode(UnitOfWorkMode mode)
        {
            switch (mode)
            {
                case UnitOfWorkMode.Disabled:
                    return new DisabledMode();                    
                case UnitOfWorkMode.SingleThreaded:
                    return new SingleThreadedMode();                    
                case UnitOfWorkMode.MultiThreaded:
                    return new MultiThreadedMode();
                default:
                    throw NewInvalidModeException(mode);
            }
        }

        private static Exception NewInvalidModeException(UnitOfWorkMode mode)
        {
            var messageFormat = ExceptionMessages.UnitOfWork_InvalidMode;
            var message = string.Format(messageFormat, mode);
            return new InternalServerErrorException(message);
        }
    }
}

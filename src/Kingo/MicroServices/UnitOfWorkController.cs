using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices
{
    internal sealed class UnitOfWorkController : UnitOfWork
    {        
        private static readonly object _NullResourceId = new object();
        
        private Dictionary<object, List<IUnitOfWorkResourceManager>> _resourceGroups;

        public UnitOfWorkController()
        {            
            _resourceGroups = new Dictionary<object, List<IUnitOfWorkResourceManager>>();
        }

        #region [====== EnlistAsync ======]

        public override Task EnlistAsync(IUnitOfWorkResourceManager resourceManager) =>
            AsyncMethod.Run(() => Enlist(resourceManager));

        private void Enlist(IUnitOfWorkResourceManager resourceManager)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException(nameof(resourceManager));
            }
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

        internal override Task FlushAsync() =>
            FlushAsync(Interlocked.Exchange(ref _resourceGroups, new Dictionary<object, List<IUnitOfWorkResourceManager>>()).Values);        

        private static Task FlushAsync(IEnumerable<List<IUnitOfWorkResourceManager>> resourceGroups) =>
            Task.WhenAll(resourceGroups.Select(FlushAsync));

        private static async Task FlushAsync(IEnumerable<IUnitOfWorkResourceManager> resourceGroup)
        {
            foreach (var resourceManager in resourceGroup.Where(resource => resource.RequiresFlush()))
            {
                await resourceManager.FlushAsync();
            }
        }

        #endregion

        #region [====== Decorate ======]

        internal override UnitOfWork Decorate() =>
            new UnitOfWorkDecorator(this);

        #endregion
    }
}

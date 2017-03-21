using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    internal sealed class UnitOfWorkControllerImplementation : IUnitOfWorkController
    {        
        private static readonly object _NullResourceId = new object();

        private readonly IUnitOfWorkCache _cache;
        private Dictionary<object, List<IUnitOfWork>> _resourceGroups;

        public UnitOfWorkControllerImplementation(IUnitOfWorkCache cache)
        {
            _cache = cache;
            _resourceGroups = new Dictionary<object, List<IUnitOfWork>>();
        }

        public IUnitOfWorkCache Cache =>
            _cache;     

        public void Enlist(IUnitOfWork unitOfWork, object resourceId) =>
            EnlistAsync(unitOfWork, resourceId).Await();

        public Task EnlistAsync(IUnitOfWork unitOfWork, object resourceId)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            return AsyncMethod.RunSynchronously(() => EnlistUnitOfWork(unitOfWork, resourceId ?? _NullResourceId));            
        }

        private void EnlistUnitOfWork(IUnitOfWork unitOfWork, object resourceId)
        {
            List<IUnitOfWork> resourceGroup;

            if (_resourceGroups.TryGetValue(resourceId, out resourceGroup))
            {
                if (resourceGroup.Contains(unitOfWork))
                {
                    return;
                }
                resourceGroup.Add(unitOfWork);
            }
            else
            {
                _resourceGroups.Add(resourceId, new List<IUnitOfWork>() { unitOfWork });
            }
        }

        public bool RequiresFlush() =>
            _resourceGroups.Values.SelectMany(unitOfWork => unitOfWork).Any(unitOfWork => unitOfWork.RequiresFlush());

        public Task FlushAsync() =>
            FlushAsync(Interlocked.Exchange(ref _resourceGroups, new Dictionary<object, List<IUnitOfWork>>()).Values);

        private static Task FlushAsync(IEnumerable<List<IUnitOfWork>> resourceGroups) =>
            Task.WhenAll(resourceGroups.Select(FlushAsync));

        private static async Task FlushAsync(IEnumerable<IUnitOfWork> resourceGroup)
        {
            foreach (var unitOfWork in resourceGroup.Where(unit => unit.RequiresFlush()))
            {
                await unitOfWork.FlushAsync();
            }
        }
    }
}

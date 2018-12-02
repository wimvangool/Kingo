using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    internal sealed class UnitOfWorkControllerImplementation : IUnitOfWorkController
    {        
        private static readonly object _NullResourceId = new object();
        
        private Dictionary<object, List<IUnitOfWork>> _resourceGroups;

        public UnitOfWorkControllerImplementation()
        {            
            _resourceGroups = new Dictionary<object, List<IUnitOfWork>>();
        }                

        public Task EnlistAsync(IUnitOfWork unitOfWork, object resourceId)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            return Run(() => EnlistUnitOfWork(unitOfWork, resourceId ?? _NullResourceId));            
        }

        private void EnlistUnitOfWork(IUnitOfWork unitOfWork, object resourceId)
        {            
            if (_resourceGroups.TryGetValue(resourceId, out var resourceGroup))
            {
                if (resourceGroup.Contains(unitOfWork))
                {
                    return;
                }
                resourceGroup.Add(unitOfWork);
            }
            else
            {
                _resourceGroups.Add(resourceId, new List<IUnitOfWork> { unitOfWork });
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

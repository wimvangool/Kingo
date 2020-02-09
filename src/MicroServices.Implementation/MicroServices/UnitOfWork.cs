using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        #region [====== DisabledMode ======]

        private sealed class DisabledMode : IUnitOfWork
        {
            public void Enlist(IChangeTracker changeTracker) =>
                throw NewUnitOfWorkDisabledException(changeTracker ?? throw new ArgumentNullException());

            public Task SaveChangesAsync() =>
                Task.CompletedTask;

            public override string ToString() =>
                UnitOfWorkMode.Disabled.ToString();

            private static Exception NewUnitOfWorkDisabledException(IChangeTracker changeTracker)
            {
                var messageFormat = ExceptionMessages.UnitOfWork_Disabled;
                var message = string.Format(messageFormat, changeTracker.GetType().FriendlyName());
                return new InvalidOperationException(message);
            }
        }

        #endregion

        #region [====== EnabledMode ======]

        private abstract class EnabledMode : IUnitOfWork
        {
            private readonly Guid _unitOfWorkId;
            private Dictionary<string, List<IChangeTracker>> _changeTrackerGroups;

            protected EnabledMode()
            {
                _unitOfWorkId = Guid.NewGuid();
                _changeTrackerGroups = new Dictionary<string, List<IChangeTracker>>();
            }

            protected Guid UnitOfWorkId =>
                _unitOfWorkId;

            protected abstract UnitOfWorkMode Mode
            {
                get;
            }

            private int ChangeTrackerCount =>
                _changeTrackerGroups.Values.Sum(group => group.Count);

            public void Enlist(IChangeTracker changeTracker)
            {
                if (_changeTrackerGroups.TryGetValue(changeTracker.GroupId, out var changeTrackerGroup))
                {
                    if (changeTrackerGroup.Contains(changeTracker))
                    {
                        return;
                    }
                    changeTrackerGroup.Add(changeTracker);
                }
                else
                {
                    _changeTrackerGroups.Add(changeTracker.GroupId, new List<IChangeTracker> { changeTracker });
                }
            }

            public Task SaveChangesAsync() =>
                SaveChangesAsync(Interlocked.Exchange(ref _changeTrackerGroups, new Dictionary<string, List<IChangeTracker>>()).Values);

            private async Task SaveChangesAsync(IReadOnlyCollection<List<IChangeTracker>> changeTrackerGroups)
            {
                try
                {
                    await SaveChangesOfAllGroupsAsync(changeTrackerGroups);
                }
                catch
                {
                    UndoChangesOfAllGroups(changeTrackerGroups);
                    throw;
                }
            }

            protected abstract Task SaveChangesOfAllGroupsAsync(IEnumerable<List<IChangeTracker>> changeTrackerGroups);

            private void UndoChangesOfAllGroups(IEnumerable<List<IChangeTracker>> changeTrackerGroups)
            {
                foreach (var changeTrackerGroup in changeTrackerGroups)
                {
                    UndoChangesOfGroup(changeTrackerGroup, UnitOfWorkId);
                }
            }

            public override string ToString() =>
                $"{Mode} ({ChangeTrackerCount} change tracker(s) enlisted)";
        }

        #endregion

        #region [====== SingleThreadedMode ======]

        private sealed class SingleThreadedMode : EnabledMode
        {
            protected override UnitOfWorkMode Mode =>
                UnitOfWorkMode.SingleThreaded;

            protected override async Task SaveChangesOfAllGroupsAsync(IEnumerable<List<IChangeTracker>> changeTrackerGroups)
            {
                foreach (var changeTrackerGroup in changeTrackerGroups)
                {
                    await SaveChangesOfGroupAsync(changeTrackerGroup, UnitOfWorkId).ConfigureAwait(false);
                }
            }                           
        }

        #endregion

        #region [====== MultiThreadedMode ======]

        private sealed class MultiThreadedMode : EnabledMode
        {
            protected override UnitOfWorkMode Mode =>
                UnitOfWorkMode.MultiThreaded;

            protected override Task SaveChangesOfAllGroupsAsync(IEnumerable<List<IChangeTracker>> flushGroups) =>
                Task.WhenAll(flushGroups.Select(flushGroup => SaveChangesOfGroupAsync(flushGroup, UnitOfWorkId)));
        }

        #endregion

        private readonly IUnitOfWork _unitOfWork;

        private UnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }        

        public void Enlist(IChangeTracker changeTracker) =>
            _unitOfWork.Enlist(changeTracker ?? throw new ArgumentNullException(nameof(changeTracker)));

        public Task SaveChangesAsync() =>
            _unitOfWork.SaveChangesAsync();

        public override string ToString() =>
            _unitOfWork.ToString();

        private static async Task SaveChangesOfGroupAsync(IEnumerable<IChangeTracker> changeTrackerGroup, Guid unitOfWorkId)
        {
            foreach (var changeTracker in changeTrackerGroup.Where(tracker => tracker.HasChanges(unitOfWorkId)))
            {
                await changeTracker.SaveChangesAsync(unitOfWorkId).ConfigureAwait(false);
            }
        }

        private static void UndoChangesOfGroup(IEnumerable<IChangeTracker> changeTrackerGroup, Guid unitOfWorkId)
        {
            foreach (var changeTracker in changeTrackerGroup)
            {
                changeTracker.UndoChanges(unitOfWorkId);
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

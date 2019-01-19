using System;
using Unity.Lifetime;

namespace Kingo.MicroServices
{
    internal sealed class DependencyContextLifetimeManager : LifetimeManager
    {
        private readonly Guid _key;

        public DependencyContextLifetimeManager()
        {
            _key = Guid.NewGuid();
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null) =>
            DependencyContext.Current?.SetValue(_key, newValue);

        public override object GetValue(ILifetimeContainer container = null) =>
            DependencyContext.Current?.GetValue(_key);

        public override void RemoveValue(ILifetimeContainer container = null) =>
            DependencyContext.Current?.RemoveValue(_key);

        protected override LifetimeManager OnCreateLifetimeManager() =>
            new DependencyContextLifetimeManager();        
    }
}

using System;

namespace Kingo
{
    internal sealed class NullServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType) =>
            null;
    }
}

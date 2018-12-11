using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo
{
    /// <summary>
    /// Represents a really simple <see cref="IServiceProvider"/> implementation that can only
    /// get services that are non-abstract and offer a public, default constructor.
    /// </summary>
    public sealed class SimpleServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}

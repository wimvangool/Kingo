using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo
{
    internal sealed class NullServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType) =>
            null;
    }
}

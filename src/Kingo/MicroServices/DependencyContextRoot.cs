using System;
using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class DependencyContextRoot : DependencyContext
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var value in Dependencies.Values.OfType<IDisposable>())
                {
                    value.Dispose();
                }
            }
            base.Dispose(disposing);
        }        
    }
}

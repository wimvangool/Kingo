using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="IServiceProvider"/> that manages
    /// the dependencies of a <see cref="IMicroProcessor"/>.
    /// </summary>
    public interface IMicroProcessorServiceProvider : IServiceProvider, IServiceScopeFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="IServiceScope"/> that manages the lifetime
        /// of scoped dependencies.
        /// </summary>
        /// <returns>A new <see cref="IServiceScope"/>.</returns>
        new IServiceScope CreateScope();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of serializers that can be used by a processor to serialize and deserialize
    /// all kinds of (data) objects.
    /// </summary>
    public sealed class SerializerCollection : IMicroProcessorComponentCollection
    {
        #region [====== IEnumerable<MicroProcessorComponent> ======]

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            throw new NotImplementedException();

        #endregion

        #region [====== AddSpecificComponentsTo ======]

        IServiceCollection IMicroProcessorComponentCollection.AddSpecificComponentsTo(IServiceCollection services) =>
            throw new NotImplementedException();

        #endregion
    }
}

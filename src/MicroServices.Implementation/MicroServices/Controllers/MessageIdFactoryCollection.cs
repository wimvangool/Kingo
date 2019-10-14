using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a collection of <see cref="MessageIdFactoryType" /> types.
    /// </summary>
    public sealed class MessageIdFactoryCollection : MicroProcessorComponentCollection<MessageIdFactoryType>
    {
        #region [====== Add ======]

        /// <inheritdoc />
        protected override bool IsComponentType(MicroProcessorComponent component, out MessageIdFactoryType componentType) =>
            MessageIdFactoryType.IsMessageIdFactory(component, out componentType);

        #endregion

        #region [====== AddSpecificComponentsTo ======]

        /// <inheritdoc />
        public override IServiceCollection AddSpecificComponentsTo(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            return services.AddSingleton<IMessageIdFactory>(new MessageIdFactory(new Dictionary<Type, IMessageIdFactory>()));
        }

        #endregion
    }
}

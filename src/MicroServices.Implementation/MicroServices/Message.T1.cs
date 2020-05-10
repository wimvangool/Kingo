using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Schema;
using System.Xml.Xsl;
using Kingo.MicroServices.DataAnnotations;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    [Serializable]
    internal abstract class Message<TContent> : IMessage<TContent>
    {
        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({Kind} | {Direction} | {Id})";

        #region [====== Kind & Direction ======]

        public abstract MessageKind Kind
        {
            get;
        }

        public abstract MessageDirection Direction
        {
            get;
        }

        #endregion

        #region [====== Id & Correlation ======]

        public abstract string Id
        {
            get;
        }

        public abstract string CorrelationId
        {
            get;
        }

        public abstract Message<TContent> CorrelateWith(IMessage message);

        #endregion

        #region [====== DeliveryTime ======]

        public abstract DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        public abstract Message<TContent> DeliverAt(DateTimeOffset? deliveryTime);

        #endregion

        #region [====== Content & Conversion ======]

        object IMessage.Content =>
            Content;

        public abstract TContent Content
        {
            get;
        }

        IMessage<TOther> IMessage.ConvertTo<TOther>() =>
            ConvertTo<TOther>();

        public Message<TOther> ConvertTo<TOther>()
        {
            if (TryConvertTo<TOther>(out var message))
            {
                return message;
            }
            throw NewConversionFailedException(typeof(TOther), Content.GetType());
        }

        bool IMessage.TryConvertTo<TOther>(out IMessage<TOther> message)
        {
            if (TryConvertTo(out Message<TOther> convertedMessage))
            {
                message = convertedMessage;
                return true;
            }
            message = null;
            return false;
        }

        public abstract bool TryConvertTo<TOther>(out Message<TOther> message);

        private static Exception NewConversionFailedException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.Message_ConversionFailed;
            var message = string.Format(messageFormat, actualType.FriendlyName(), expectedType.FriendlyName());
            return new InvalidCastException(message);
        }

        #endregion

        #region [====== Validation ======]

        public abstract Message<TContent> Validate(IServiceProvider serviceProvider);

        #endregion
    }
}

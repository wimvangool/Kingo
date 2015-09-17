using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.BuildingBlocks.Messaging
{    
    [Serializable]
    internal sealed class InvalidEventException : ArgumentException
    {
        private const string _InvalidEventKey = "_invalidEvent";
        private const string _ErrorInfoCollectionKey = "_errorInfoCollection";

        internal readonly IMessage InvalidEvent;
        internal readonly IReadOnlyList<DataErrorInfo> ErrorInfoCollection;        
                
        internal InvalidEventException(string paramName, IMessage invalidEvent, string message, IReadOnlyList<DataErrorInfo> errorInfoCollection)
            : base(message, paramName)
        {            
            InvalidEvent = invalidEvent;
            ErrorInfoCollection = errorInfoCollection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private InvalidEventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InvalidEvent = (IMessage) info.GetValue(_InvalidEventKey, typeof(IMessage));
            ErrorInfoCollection = (IReadOnlyList<DataErrorInfo>) info.GetValue(_ErrorInfoCollectionKey, typeof(IReadOnlyList<DataErrorInfo>));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_InvalidEventKey, InvalidEvent);
            info.AddValue(_ErrorInfoCollectionKey, ErrorInfoCollection);
        }                
    }
}

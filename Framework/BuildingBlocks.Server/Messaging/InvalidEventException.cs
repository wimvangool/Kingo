using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.BuildingBlocks.Messaging
{    
    [Serializable]
    internal sealed class InvalidEventException : ArgumentException
    {
        private const string _InvalidEventKey = "_invalidEvent";
        private const string _ErrorInfoKey = "_errorInfo";

        internal readonly IMessage InvalidEvent;
        internal readonly DataErrorInfo ErrorInfo;        
                
        internal InvalidEventException(string paramName, IMessage invalidEvent, string message, DataErrorInfo errorInfo)
            : base(message, paramName)
        {            
            InvalidEvent = invalidEvent;
            ErrorInfo = errorInfo;
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
            ErrorInfo = (DataErrorInfo) info.GetValue(_ErrorInfoKey, typeof(DataErrorInfo));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_InvalidEventKey, InvalidEvent);
            info.AddValue(_ErrorInfoKey, ErrorInfo);
        }                
    }
}

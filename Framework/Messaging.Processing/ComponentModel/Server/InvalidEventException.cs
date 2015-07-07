using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Syztem.ComponentModel.Server
{    
    [Serializable]
    internal sealed class InvalidEventException : ArgumentException
    {
        private const string _InvalidEventKey = "_invalidEvent";
        private const string _ErrorTreeKey = "_errorTree";

        private readonly IMessage _invalidEvent;
        private readonly ValidationErrorTree _errorTree;        
                
        internal InvalidEventException(string paramName, IMessage invalidEvent, string message, ValidationErrorTree errorTree)
            : base(message, paramName)
        {            
            _invalidEvent = invalidEvent;
            _errorTree = errorTree;
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
            _invalidEvent = (IMessage) info.GetValue(_InvalidEventKey, typeof(IMessage));
            _errorTree = (ValidationErrorTree) info.GetValue(_ErrorTreeKey, typeof(ValidationErrorTree));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_InvalidEventKey, _invalidEvent);
            info.AddValue(_ErrorTreeKey, _errorTree);
        }

        /// <summary>
        /// The invalid event.
        /// </summary>
        public IMessage InvalidEvent
        {
            get { return _invalidEvent; }
        }

        /// <summary>
        /// If specified, contains all the validation-errors of the <see cref="FunctionalException.FailedMessage" />.
        /// </summary>
        public ValidationErrorTree ErrorTree
        {
            get { return _errorTree; }
        }
    }
}

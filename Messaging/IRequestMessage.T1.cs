using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    /// <summary>
    /// Represent a message that can validate itself.
    /// </summary>
    public interface IRequestMessage<out TMessage> : IRequestMessage, IMessage<TMessage>
        where TMessage : class, IRequestMessage<TMessage>
    {
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether or not the copy should be readonly.</param>
        /// <returns>
        /// A copy of this message. If <paramref name="makeReadOnly"/> is <c>true</c>,
        /// all data properties of the copy will be readonly. In addition, the returned copy will be marked unchanged,
        /// even if this message is marked as changed. If the copy is readonly, the HasChanges-flag cannot be
        /// set to <c>true</c>.
        /// </returns>
        new TMessage Copy(bool makeReadOnly);
    }
}

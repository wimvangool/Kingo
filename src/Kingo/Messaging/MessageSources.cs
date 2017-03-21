using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a (collection of) source(s) a message can originate from.
    /// </summary>
    [Flags]
    public enum MessageSources
    {
        /// <summary>
        /// Represents a source that has not been defined.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates a message represents a query input message.
        /// </summary>
        Query = 1,

        /// <summary>
        /// Indicates a message was passed directly to the <see cref="IMicroProcessor" />.
        /// </summary>
        InputStream = 2,

        /// <summary>
        /// Indicates a message originates from the <see cref="IMicroProcessorContext.OutputStream" />.
        /// </summary>
        OutputStream = 4,

        /// <summary>
        /// Indicates a message originates from the <see cref="IMicroProcessorContext.MetadataStream" />.
        /// </summary>
        MetadataStream = 8,

        /// <summary>
        /// Represents all streams (input, output and metadata stream).
        /// </summary>
        AnyStream = InputStream | OutputStream | MetadataStream,

        /// <summary>
        /// Represents all sources.
        /// </summary>
        All = Query | AnyStream
    }
}

using System;

namespace YellowFlare.MessageProcessing
{    
    /// <summary>
    /// This attribute can be used to configure how a certain unit of work should be flushed.
    /// </summary>
    /// <remarks>
    /// This attribute should be put on classes that implement the <see cref="IUnitOfWork"/>-interface
    /// for it to have any effect.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class FlushHintAttribute : Attribute
    {              
        /// <summary>
        /// Gets or sets the provided identifier of the unit of work.
        /// </summary>
        public string Group
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an indicator whether or not the specified unit of work must always be flushed on the same thread
        /// as it was created. Default is <c>false</c>.
        /// </summary>
        public bool ForceSynchronousFlush
        {
            get;
            set;
        }
    }
}


namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// The options that can be specified when a property of a <see cref="Message" /> has changed.
    /// </summary>    
    public enum MessageChangedOption
    {
        /// <summary>
        /// Indicates that no further action is required.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the Message must be marked as changed.
        /// </summary>
        MarkAsChanged = 1,        

        /// <summary>
        /// Indicates that the Message must be marked as changed and must be validated.
        /// </summary>
        MarkAsChangedAndValidate = 2
    }
}

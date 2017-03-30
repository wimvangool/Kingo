using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// When implemented by a class, represent a certain member that can be validated.
    /// </summary>
    public interface IMember
    {
        /// <summary>
        /// The display name of this member.
        /// </summary>
        string DisplayName
        {
            get;
        }

        /// <summary>
        /// The full name of this member.
        /// </summary>
        string FullName
        {
            get;
        }

        /// <summary>
        /// The name of this member.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// The type of this member's value.
        /// </summary>
        Type Type
        {
            get;
        }        
    }
}

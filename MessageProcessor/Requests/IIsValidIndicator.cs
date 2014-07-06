using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a component that can indicate whether or not it is valid.
    /// </summary>
    public interface IIsValidIndicator : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when <see cref="IsValid" /> changed.
        /// </summary>
        event EventHandler IsValidChanged;

        /// <summary>
        /// Indicates whether or not the component is valid.
        /// </summary>
        bool IsValid
        {
            get;
        }
    }
}

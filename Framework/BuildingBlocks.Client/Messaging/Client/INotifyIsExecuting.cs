using System;
using System.Windows.Input;

namespace Kingo.BuildingBlocks.Messaging.Client
{
    /// <summary>
    /// Represents a request made from a client that lies on top of a regular <see cref="IRequestDispatcher">Request</see>.
    /// </summary>
    public interface INotifyIsExecuting : ICommand, INotifyIsBusy, INotifyIsValid
    {
        /// <summary>
        /// Indicates that <see cref="IsExecuting" /> has changed.
        /// </summary>
        event EventHandler IsExecutingChanged;

        /// <summary>
        /// Indicates whether or not one or more executions for this <see cref="IRequestDispatcher" /> are running.
        /// </summary>
	    bool IsExecuting
	    {
	        get;
	    }
    }
}

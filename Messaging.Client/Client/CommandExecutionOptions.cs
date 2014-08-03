﻿namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a set of options that defines the behavior of a <see cref="RequestDispatcherCommand{T, S}" />.
    /// </summary>
    [Flags]
    public enum CommandExecutionOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that the request is allowed to have nested (in case of synchronous) or
        /// parrallel (in case of asynchronous) executions.
        /// </summary>
        AllowMultipleExecutions = 1,

        /// <summary>
        /// Forces the request to execute synchronously.
        /// </summary>
        ExecuteSynchronously = 2
    }
}
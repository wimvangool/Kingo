using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Represents an abstract scope in code in which certain changes are made that can be committed or rolled back.    
    /// </summary>
    /// <remarks>
    /// <para>
    /// When implemented by a class, the <see cref="ITransactionalScope" /> is entered as soon as the scope is created
    /// end exitted when <see cref="IDisposable.Dispose">Dispose()</see> is called. The <see cref="ITransactionalScope.Complete()" />
    /// method is responsible for committing the changes that were made inside the scope.
    /// </para>
    /// <para>
    /// This design has of course been inspired by the design of the <see cref="TransactionScope" /> class.
    /// </para>
    /// </remarks>
    public interface ITransactionalScope : IDisposable
    {
        /// <summary>
        /// Completes the scope by flushing all registered wrappers.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The scope has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The scope has already been completed.
        /// </exception>
        void Complete();
    }
}

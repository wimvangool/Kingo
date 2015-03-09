﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace System.ComponentModel.Server.Transactions
{
    /// <summary>
    /// When implemented by a class, represents a factory for <see cref="TransactionScope">TransactionScopes</see>.
    /// </summary>
    public interface ITransactionScopeFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="TransactionScope" />.
        /// </summary>
        /// <returns>A new <see cref="TransactionScope" />.</returns>
        TransactionScope CreateTransactionScope();
    }
}
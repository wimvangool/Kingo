using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// Represents a collection of validation errors.
    /// </summary>
    public sealed class ValidationResultCollection
    {
        private readonly ValidationResult[] _results;

        internal ValidationResultCollection(IEnumerable<ValidationResult> results)
        {
            _results = results.ToArray();
        }
    }
}

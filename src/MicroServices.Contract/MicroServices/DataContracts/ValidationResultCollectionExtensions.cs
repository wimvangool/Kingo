using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kingo.MicroServices.DataContracts
{    
    internal static class ValidationResultCollectionExtensions
    {                    
        public static IEnumerable<ValidationResult> ErrorsOnly(this IEnumerable<ValidationResult> results) =>
            results.Where(result => result != ValidationResult.Success);
    }
}

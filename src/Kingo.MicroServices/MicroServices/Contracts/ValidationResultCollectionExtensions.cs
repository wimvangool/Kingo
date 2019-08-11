using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Kingo.MicroServices.Contracts
{    
    internal static class ValidationResultCollectionExtensions
    {                    
        public static IEnumerable<ValidationResult> ErrorsOnly(this IEnumerable<ValidationResult> results) =>
            results.Where(result => result != ValidationResult.Success);                             
    }
}

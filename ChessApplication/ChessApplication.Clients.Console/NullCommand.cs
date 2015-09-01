using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceComponents.Threading;

namespace ServiceComponents.ChessApplication
{
    internal sealed class NullCommand : IUserCommand
    {
        public Task<bool> ExecuteWithAsync(IUserCommandProcessor processor)
        {
            return AsyncMethod.Value(true);
        }
    }
}

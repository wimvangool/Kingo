using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ChessApplication
{
    internal interface IUserCommand
    {
        Task<bool> ExecuteWithAsync(IUserCommandProcessor processor);
    }
}

using System.Threading.Tasks;

namespace Kingo.ChessApplication
{
    internal interface IUserCommand
    {
        Task<bool> ExecuteWithAsync(IUserCommandProcessor processor);
    }
}

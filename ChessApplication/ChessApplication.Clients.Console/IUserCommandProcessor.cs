using System.Threading.Tasks;

namespace Kingo.ChessApplication
{
    internal interface IUserCommandProcessor
    {        
        Task<bool> ExecuteCommandAsync(string name, UserCommandArgumentStack arguments);
    }
}

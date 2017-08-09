using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    public sealed class MovePieceHandler : IMessageHandler<MovePieceCommand>
    {
        private readonly IGameRepository _games;

        public MovePieceHandler(IGameRepository games)
        {            
            _games = games;
        }

        public async Task HandleAsync(MovePieceCommand message, IMicroProcessorContext context)
        {
            var from = Square.Parse(message.From);
            var to = Square.Parse(message.To);
            var game = await _games.GetByKeyAsync(message.GameId);
            
            game.MovePiece(Session.Current.UserId, from, to);
        }
    }
}

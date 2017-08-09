using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class ActivePlayer : Player
    {
        private readonly Game _game;
        private readonly Guid _playerId;
        private readonly ColorOfPiece _color;

        public ActivePlayer(Game game, Guid playerId, ColorOfPiece color)
        {
            _game = game;
            _playerId = playerId;
            _color = color;
        }

        protected override Guid PlayerId =>
            _playerId;

        public override Player SwitchTurn() =>
            new PassivePlayer(_game, _playerId, _color);

        public override void MovePiece(Square from, Square to) =>
            _game.Board.MovePiece(Move.Calculate(from, to), _color);

        public override void PromotePawn(TypeOfPiece promoteTo) =>
            _game.Board.PromotePawn(promoteTo, _color);

        public override void Forfeit()
        {
            //EventBus.Publish(new GameForfeitedEvent());
            throw new NotImplementedException();
        }        
    }
}

using System;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class PassivePlayer : Player
    {
        private readonly Game _game;
        private readonly Guid _playerId;
        private readonly ColorOfPiece _color;

        public PassivePlayer(Game game, Guid playerId, ColorOfPiece color)
        {
            _game = game;
            _playerId = playerId;
            _color = color;
        }

        protected override Guid PlayerId =>
            _playerId;

        public override Player SwitchTurn() =>
            new ActivePlayer(_game, _playerId, _color);

        public override void MovePiece(Square from, Square to)
        {
            throw NewNotPlayersTurnException(_playerId);
        }

        public override void PromotePawn(TypeOfPiece promoteTo)
        {
            throw NewNotPlayersTurnException(_playerId);
        }

        public override void Forfeit()
        {
            throw NewNotPlayersTurnException(_playerId);
        }

        private static Exception NewNotPlayersTurnException(Guid playerId)
        {
            var messageFormat = ExceptionMessages.Game_NotPlayersTurn;
            var message = string.Format(messageFormat, playerId);
            return new IllegalOperationException(message);
        }
    }
}

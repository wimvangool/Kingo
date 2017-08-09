using System;

namespace Kingo.Samples.Chess.Games
{
    internal abstract class Player
    {        
        protected abstract Guid PlayerId
        {
            get;
        }

        public bool IsPlayer(Guid playerId) =>
            PlayerId == playerId;

        public abstract Player SwitchTurn();

        public abstract void MovePiece(Square from, Square to);

        public abstract void PromotePawn(TypeOfPiece promoteTo);

        public abstract void Forfeit();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal abstract class Player
    {        
        protected abstract Guid PlayerId
        {
            get;
        }

        public bool IsPlayer(Guid playerId)
        {
            return PlayerId == playerId;
        }

        public abstract Player SwitchTurn();

        public abstract void MovePiece(Square from, Square to);

        public abstract void Forfeit();
    }
}

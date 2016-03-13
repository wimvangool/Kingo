using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Games
{
    public enum GameState
    {
        Normal,

        Check,

        CheckMate,

        StaleMate,

        Forfeited
    }
}

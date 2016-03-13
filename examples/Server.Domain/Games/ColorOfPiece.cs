namespace Kingo.Samples.Chess.Games
{
    internal enum ColorOfPiece
    {
        White,

        Black
    }

    internal static class ColorOfPieceExtensions
    {
        public static ColorOfPiece Invert(this ColorOfPiece color)
        {
            return color == ColorOfPiece.White ? ColorOfPiece.Black : ColorOfPiece.White;
        }
    }
}

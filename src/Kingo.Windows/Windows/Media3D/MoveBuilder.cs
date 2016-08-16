using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    internal sealed class MoveBuilder
    {
        private readonly double _moveSpeed;
        private readonly List<Vector3D> _moves;

        public MoveBuilder(double moveSpeed)
        {
            _moveSpeed = moveSpeed;
            _moves = new List<Vector3D>();
        }

        public int MoveCount => _moves.Count;

        public void AddMove(Vector3D move)
        {
            _moves.Add(move);
        }

        public Vector3D BuildMove()
        {
            var compositeMove = new Vector3D();

            foreach (var move in _moves)
            {
                compositeMove += move;
            }
            if (IsValidMove(compositeMove))
            {
                compositeMove.Normalize();
                compositeMove *= _moveSpeed;

                return compositeMove;
            }
            return NoMove;            
        }

        public static Vector3D NoMove = new Vector3D();

        private static bool IsValidMove(Vector3D move)
        {
            return Math.Round(move.Length, 6) > 0;
        }
    }
}

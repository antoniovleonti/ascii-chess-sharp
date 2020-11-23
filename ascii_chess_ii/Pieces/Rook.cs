using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    class Rook : IPiece
    {
        /*-------------------------------------LAZY SINGLETON IMPLEMENTATION-*/

        private static readonly Lazy<Rook> lazy =
            new Lazy<Rook>( () => new Rook() );

        public static Rook Instance { get { return lazy.Value; } }

        private Rook() { }

        /*-----------------------------------------------------------METHODS-*/

        public void MakeMove(Board board, Move move, int player) { }

        public (List<Pos2> m, List<Pos2> c) ListMoves(Board board, Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int dy, dx;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    dy = i * (1 - 2 * j);
                    dx = (i == 0 ? 1 : 0) * (1 - 2 * j);

                    var (tmpm, tmpc) = IPiece.CastMoves(board, pos, dy, dx, player);

                    maneuvers.AddRange(tmpm);
                    captures.AddRange(tmpc);
                }
            }
            return (maneuvers, captures);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    class Bishop : IPiece
    {
        /*-------------------------------------LAZY SINGLETON IMPLEMENTATION-*/

        private static readonly Lazy<Bishop> lazy =
            new Lazy<Bishop>( () => new Bishop() );

        public static Bishop Instance { get { return lazy.Value; } }

        private Bishop() { }
        
        /*-----------------------------------------------------------METHODS-*/

        public (List<Pos2> m, List<Pos2> c) ListMoves(Board board, Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int dy, dx;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    dy = 1 - 2 * i; // every combination of 1 & -1
                    dx = 1 - 2 * j;

                    var (tmpm, tmpc) = IPiece.CastMoves(board, pos, dy, dx, player);

                    maneuvers.AddRange(tmpm);
                    captures.AddRange(tmpc);
                }
            }
            return (maneuvers, captures);
        }
    }
}

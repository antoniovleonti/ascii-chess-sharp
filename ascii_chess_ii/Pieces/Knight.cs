using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    class Knight : IPiece
    {
        /*-------------------------------------LAZY SINGLETON IMPLEMENTATION-*/

        private static readonly Lazy<Knight> lazy =
            new Lazy<Knight>( () => new Knight() );

        public static Knight Instance { get { return lazy.Value; } }

        private Knight() { }

        /*-----------------------------------------------------------METHODS-*/

        public void MakeMove(Board board, Move move, int player) { }

        public (List<Pos2> m, List<Pos2> c) ListMoves(Board board, Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int[,] dydx =
            {
                { 2,-1},{ 2, 1},{-2,-1},{-2, 1},
                { 1,-2},{ 1, 2},{-1,-2},{-1, 2}
            };

            int y, x;
            for (int i = 0; i < (dydx.Length / dydx.Rank); i++)
            {
                y = pos.y + dydx[i, 0];
                x = pos.x + dydx[i, 1];

                // if this is a valid spot to put the knight
                if (y >= 0 && y < 8 // within bounds
                    && x >= 0 && x < 8 // ^^^
                    && Math.Sign(board.value[y, x]) != player) // not own piece
                {
                    if (Math.Sign(board.value[y, x]) == -player)
                    {
                        captures.Add(new Pos2(y, x));
                    }
                    else maneuvers.Add(new Pos2(y, x));
                }
            }
            return (maneuvers, captures);
        }
    }
}

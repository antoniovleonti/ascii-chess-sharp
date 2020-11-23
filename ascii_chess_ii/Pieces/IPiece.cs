using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    interface IPiece
    {
        public abstract (List<Pos2> m, List<Pos2> c) ListMoves(Board board, Pos2 pos, int player);

        public abstract void MakeMove(Board board, Move move, int player);

        // static methods

        protected static (List<Pos2> m, List<Pos2> c)
            CastMoves(Board board, Pos2 pos, int dx, int dy, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int y, x;
            for (int i = 1; true; i++)
            {
                y = pos.y + dy * i;
                x = pos.x + dx * i;

                if (y >= 0 && y < 8 // within bounds
                    && x >= 0 && x < 8 // ^^^
                    && Math.Sign(board.value[y, x]) != player) // is not friendly piece
                {
                    if (Math.Sign(board.value[y, x]) == -player)
                    {
                        captures.Add(new Pos2(y, x));
                        return (maneuvers, captures);
                    }
                    else maneuvers.Add(new Pos2(y, x));
                }
                else return (maneuvers, captures);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    class King : IPiece
    {
        /*-------------------------------------LAZY SINGLETON IMPLEMENTATION-*/

        private static readonly Lazy<King> lazy =
            new Lazy<King>( () => new King() );

        public static King Instance { get { return lazy.Value; } }

        private King() { }

        /*-----------------------------------------------------------METHODS-*/

        public (List<Pos2> m, List<Pos2> c) ListMoves(Board board, Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            // first find all normal moves
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int y = pos.y + dy;
                    int x = pos.x + dx;

                    if (y >= 0 && y < 8 // within bounds
                    && x >= 0 && x < 8 // ^^^
                    && Math.Sign(board.Value[y, x]) != player) // or is not friendly piece
                    {
                        if (Math.Sign(board.Value[y, x]) == -player)
                        {
                            captures.Add(new Pos2(y, x));
                        }
                        else maneuvers.Add(new Pos2(y, x));
                    }
                }
            }

            // now find castling moves
            // TODO: implement chess960 castling rules (more general)
            if (board.Touch[pos.y, pos.x] == 0 // untouched king
            && !board.PosIsHitBy(pos, new uint[] { Pieces.KING }, player)) // not in check
            {
                for (int i = 0; i < 2; i++)
                {
                    int dx = 1 - 2 * i; // -1 or 1

                    if (board.Touch[pos.y, (int)(4 + 3.5f * dx)] == 0) // untouched rook
                    {
                        bool flag = false;

                        // make sure squares are open
                        for (int j = 3 + dx; j > 0 && j < 7; j += dx)
                        {
                            if (board.Value[pos.y, j] != 0)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag) continue;

                        maneuvers.Add(new Pos2(pos.y, 3 + 2 * dx));
                    }
                }
            }

            return (maneuvers, captures);
        }
    }
}

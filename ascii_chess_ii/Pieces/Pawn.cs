using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    class Pawn : IPiece
    {
        /*-------------------------------------LAZY SINGLETON IMPLEMENTATION-*/

        private static readonly Lazy<Pawn> lazy =
            new Lazy<Pawn>( () => new Pawn() );

        public static Pawn Instance { get { return lazy.Value; } }

        private Pawn() { }

        /*-----------------------------------------------------------METHODS-*/

        public (List<Pos2> m, List<Pos2> c) ListMoves(Board board, Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            // first determine maneuvers available to the pawn
            int y = pos.y + player;
            int x = pos.x;

            if (y < 8 && y >= 0
            && Math.Sign(board.Value[y, x]) == 0) // or is empty
            {
                // it can move forward 1
                maneuvers.Add(new Pos2(y, x));

                // if pawn is at home & nothing is 2 squares ahead
                y += player;
                if (board.Touch[pos.y, x] == 0
                && Math.Sign(board.Value[y, x]) == 0) // or is empty
                {
                    maneuvers.Add(new Pos2(y, x));
                }
            }

            // now determine captures available to the pawn
            y = pos.y + player;
            for (int i = 0; i < 2; i++)
            {
                x = pos.x + 1 - 2 * i;

                // check if it's possible to capture in this direction
                if (x >= 0 && x < 8
                && y >= 0 && y < 8
                && (Math.Sign(board.Value[y, x]) == -player // other player's piece                              
                    || (pos.y == (int)(3.5f + player) // or is en passant
                        && board.Value[pos.y, x] == -player
                        && board.Touch[pos.y, x] == 1
                        && board.Touch[pos.y + player * 2, x] == 1)))
                {
                    captures.Add(new Pos2(y, x));
                }
            }
            return (maneuvers, captures);
        }
    }
}

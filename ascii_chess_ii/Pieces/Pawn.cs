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

        public void MakeMove(Board board, Move move, int player)
        {
            // enable pawn promotions
            if (move.dst.y == (int)(4 + (3.5 * player)))
            {
                board.value[move.src.y, move.src.x] = (int)Pieces.QUEEN * player;
            }
        }

        public (List<Pos2> m, List<Pos2> c) ListMoves(Board board, Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            // first determine maneuvers available to the pawn
            int y = pos.y + player;
            int x = pos.x;

            if (y < 8 && y >= 0
                && Math.Sign(board.value[y, x]) == 0) // or is empty
            {
                // it can move forward 1
                maneuvers.Add(new Pos2(y, x));

                // if pawn is at home & nothing is 2 squares ahead
                y += player;
                if (board.touch[pos.y, x] == 0
                    && Math.Sign(board.value[y, x]) == 0) // or is empty
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
                    && y >= 0 && y < 8)
                {
                    // capture other player's piece directly
                    if (Math.Sign(board.value[y, x]) == -player)
                    {
                        captures.Add(new Pos2(y, x));
                    }
                    // capture via en passant (considered maneuver)
                    if (pos.y == (int)(3.5f + player) 
                        && board.value[pos.y, x] == -player
                        && board.touch[pos.y, x] == 1
                        && board.touch[pos.y + player * 2, x] == 1)
                    {
                        maneuvers.Add(new Pos2(y, x));
                    }
                }
            }
            return (maneuvers, captures);
        }
    }
}

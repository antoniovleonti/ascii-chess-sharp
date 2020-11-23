using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    class Game
    {
        private int player;
        public int Player { get { return player; } }
        public Board board { get; }

        public Game()
        {
            uint[,] _t = new uint[8, 8];
            int[,] _v =
            {   
                { 4, 2, 3, 6, 5, 3, 2, 4},
                { 1, 1, 1, 1, 1, 1, 1, 1},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                {-1,-1,-1,-1,-1,-1,-1,-1},
                {-4,-2,-3,-6,-5,-3,-2,-4}   
            };

            board = new Board(_v, _t);
            player = 1;
        }
        public Game(Board b, int startingPlayer)
        {
            board = b;
            player = startingPlayer;
        }

        public int? TryMove(Move input)
        {
            if(input == null) return null;
            
            if (board.MoveIsPseudoLegal(input, player)
            &&  !board.MoveIsSuicide(input, player)  )
            {
                board.MakeMove(input, player);

                player *= -1;

                // if the game is now over (opposing player has no moves)
                if (board.PlayerIsTrapped(player))
                {
                    // (if null move is suicide, player is in check)
                    return board.MoveIsSuicide(null, player) ? -player : 0;
                }
            }
            return null;
        }
    }
}

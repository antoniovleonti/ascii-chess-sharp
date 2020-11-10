using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    class Manager
    {
        public static int PlayChessGame(Board board)
        {
            // the game begins. player switches between 1 & -1
            for(int player = 1; true; player *= -1)
            {
                // print the board
                Console.WriteLine(board.ToString(player));

                // check for end of game
                if(board.PlayerIsTrapped(player))
                {
                    for (int i = 0; i <= 35; i++) Console.Write("*");

                    // (if null move is suicide, player is in check)
                    if (board.MoveIsSuicide(null, player))
                    {
                        Console.WriteLine($"\n\n\tCHECKMATE! {(player==1?"White":"Black")} wins!\n");
                    }
                    else
                    {
                        Console.WriteLine($"\n\n\tSTALEMATE! It's a draw!\n");
                    }
                }

                // otherwise, the game continues.

                // prompt input
                Console.Write($"\n\t{(player==1?"White":"Black")}\'s turn: > ");

                Move input = ReadMove(Console.ReadLine());

                if(input == null)
                {
                    Console.WriteLine("\n\t (?) Invalid input! Try again.");

                    player *= -1;
                    continue;
                }
                
                if (board.MoveIsPseudoLegal(input, player)
                &&  !board.MoveIsSuicide(input, player)  )
                {
                    board.MakeMove(input, player);
                }
                else
                {
                    Console.WriteLine("\n\t (!) Illegal move! Try again.");

                    player *= -1;
                    continue;
                }
            }
        }

        private static Move ReadMove(string s)
        {
            if (s.Length < 4) return null;
            
            string abcs = "hgfedcba";
            string nums = "12345678";
            string prom = "NBRQ";

            int[] t = {-1, -1, -1, -1, -1};

            // get coords
            for(int i = 0; i < 4; i++)
            {
                t[i] = abcs.IndexOf(s[i]);
                if (t[i] == -1) t[i] = nums.IndexOf(s[i]);

                if (t[i] == -1) return null;
            }
            // get modifier
            if(s.Length > 4)
            {
                t[4] = prom.IndexOf(s.ToLower()[4]);
                if (t[4] == -1) return null;
            }

            Move move = new Move(   new Pos2(t[1], t[0]),
                                    new Pos2(t[3], t[2]),
                                    t[4]);

            return move;
        }
    }
}

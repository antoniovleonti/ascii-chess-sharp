using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ASCII_Chess_II
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            int? result = null;
            int lastPlayer;

            while (result == null)
            {
                // print board state
                Console.WriteLine(game.Board.ToString(game.Player));

                string s = Console.ReadLine(); // get player input
                Move input = ReadMove(s);
                
                lastPlayer = game.Player; // get current player

                if (input == null)
                {
                    Console.WriteLine("\t(?) Cannot parse move!");
                }

                // try to make this move
                result = game.TryMove(input);

                // if it's not the other player's turn now, move was illegal.
                if (game.Player == lastPlayer)
                {
                    Console.WriteLine("\t(!) Illegal move!");
                }
            }
        }

        private static Move ReadMove(string s)
        {
            if (s.Length < 4) return null;

            string abcs = "hgfedcba";
            string nums = "12345678";

            int[] t = { -1, -1, -1, -1, -1 };

            // get coords
            for (int i = 0; i < 4; i++)
            {
                t[i] = abcs.IndexOf(s[i]);
                if (t[i] == -1) t[i] = nums.IndexOf(s[i]);

                if (t[i] == -1) return null;
            }

            Move move = 
                new Move(new Pos2(t[1], t[0]), new Pos2(t[3], t[2]));

            return move;
        }
    }
}

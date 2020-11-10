using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ASCII_Chess_II
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            // set up piece state
            int[,] piece =
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

            // set up touch state
            uint[,] touch = new uint[8, 8];

            // new board using pieces & touch state
            Board board = new Board(piece, touch);

            Manager.PlayChessGame(board);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_Chess_II
{
    // maps each piece to its label ("KING", "PAWN", etc) and IPiece implementation
    static class Pieces
    {
        public const uint PAWN   = 1; // ID of each piece on board.
        public const uint KNIGHT = 2; // each can be positive  (white)
        public const uint BISHOP = 3; // or negative (black).
        public const uint ROOK   = 4;
        public const uint QUEEN  = 5;
        public const uint KING   = 6;

        public static Dictionary<uint, IPiece> dict = new Dictionary<uint, IPiece>()
        {
            { PAWN,   Pawn.Instance   },
            { KNIGHT, Knight.Instance },
            { BISHOP, Bishop.Instance },
            { ROOK,   Rook.Instance   },
            { QUEEN,  Queen.Instance  },
            { KING,   King.Instance   },
        };
    }
}

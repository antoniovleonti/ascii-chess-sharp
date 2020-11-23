using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_Chess_II
{
    class Board
    {
        /*--------------------------------------------------------ATTRIBUTES-*/

        
        public int[,] value { get; } // integer value of piece present in each square
        public uint[,] touch { get; } // # of moves since square has been touched
        // "touched" means a piece moving to or from the square. starts at 0


        /*-----------------------------------------------------------METHODS-*/


        /*-CONSTRUCTORS------------------------------------------------------*/

        public Board(int[,] _value, uint[,] _touch) // new constructor
        {
            value = new int[8, 8];
            touch = new uint[8, 8];

            Array.Copy(_value, value, 64);
            Array.Copy(_touch, touch, 64);
        }

        public Board(Board other) // copy constructor
        {
            value = new int[8, 8];
            touch = new uint[8, 8];

            Array.Copy(other.value, value, 64);
            Array.Copy(other.touch, touch, 64);
        }


        /*-PIECE MOVEMENT FUNCTIONS------------------------------------------*/


        // plays move on the board & handles special cases such as castling
        public void MakeMove(Move move, int player)
        {
            if (move == null) throw new ArgumentNullException();

            uint p = (uint)Math.Abs(value[move.src.y, move.src.x]);

            // special cases
            if (Pieces.dict.ContainsKey(p))
            {
                Pieces.dict[p].MakeMove(this, move, player);
            }

            // perform move
            value[move.dst.y, move.dst.x] = value[move.src.y, move.src.x];
            value[move.src.y, move.src.x] = 0;

            // increment touch
            for(int i = 0; i < 8; i++)
                for(int j = 0; j < 8; j++) 
                    if (touch[i, j] > 0) touch[i, j]++;

            // remember these squares were touched
            touch[move.dst.y, move.dst.x] = touch[move.src.y, move.src.x] = 1;
        }


        /*-BOARD STATE QUERY FUNCTIONS---------------------------------------*/

        public bool PosIsHitBy(Pos2 pos, uint[] ignoredPieces, int player)
        {
            if (pos == null) throw new ArgumentNullException();
            if (ignoredPieces == null) ignoredPieces = new uint[] { };

            // iterate through each piece type
            foreach (KeyValuePair<uint, IPiece> kvp in Pieces.dict)
            {
                // check if piece is included
                if (ignoredPieces.Contains(kvp.Key)) continue;

                // get all captures possible of piece from pos
                List<Pos2> captures = kvp.Value.ListMoves(this, pos, player).c;

                // check all potential captures here
                foreach (Pos2 dst in captures)
                {
                    // if this is the right piece to attack pos
                    if (Math.Abs(value[dst.y, dst.x]) == kvp.Key) return true;
                }
            }
            return false;
        }

        public bool PlayerIsTrapped(int player)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int p = value[i, j];

                    // if piece does not belong to player, move on
                    if (Math.Sign(p) != player) continue;

                    List<Pos2>[] sqrs = new List<Pos2>[2]; // destination squares
                    Pos2 src = new Pos2(i, j); // current piece's starting position

                    // get all pseudo-legal moves for current piece
                    (sqrs[0], sqrs[1]) =
                        Pieces.dict[(uint)Math.Abs(p)].ListMoves(this, src, player);

                    foreach (List<Pos2> l in sqrs) // for each move type
                    {
                        // try to find a non suicidal move
                        if (l.Exists(d => !MoveIsSuicide(new Move(src, d), player)))
                            return false;
                    }
                }
            }
            // if you've checked every move and none work,
            return true;
        }


        /*-MOVE LEGALITY EVALUATION FUNCTIONS--------------------------------*/

        public bool MoveIsPseudoLegal(Move move, int player)
        {
            if (move == null) throw new ArgumentNullException();

            // get values at each board position
            int[] p = { value[move.src.y, move.src.x],
                        value[move.dst.y, move.dst.x] };

            // if you're trying to move the other player's pieces (or no piece)...
            if (Math.Sign(p[0]) != player) return false;

            // get all possible moves with piece
            var (maneuvers, captures) =
                Pieces.dict[(uint)Math.Abs(p[0])].ListMoves(this, move.src, player);

            // decide whether to check captures or maneuvers
            List<Pos2> list = (p[1] == 0) ? maneuvers : captures;

            // try to find desired square in list
            return list.Exists(p => p.Equals(move.dst));
        }

        public bool MoveIsSuicide(Move move, int player)
        {
            // allow null move parameter to mean no move
            move ??= new Move(new Pos2(0, 0), new Pos2(0, 0));

            Board test = new Board(this); // copy board state
            test.MakeMove(move, player); // make move on new board

            // find kind in both variations
            Pos2[] pos = new Pos2[2];

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (value[i, j] == Pieces.KING * player)
                        pos[0] = new Pos2(i, j);

                    if (test.value[i, j] == Pieces.KING * player)
                        pos[1] = new Pos2(i, j);

                    if (pos[0] != null && pos[1] != null) break; 
                }
            }

            do // (must happen at least once)
            {
                pos[0].y += Math.Sign(pos[1].y - pos[0].y); // interpolate king positions on 2nd board 
                pos[0].x += Math.Sign(pos[1].x - pos[0].x);  // (if king is stationary do nothing)

                // if king is hit at any point during his journey
                if (test.PosIsHitBy(pos[0], null, player))
                    return true;
            }
            while (pos[0].y != pos[1].y || pos[0].x != pos[1].x);

            return false;
        }


        /*-IO FUNCTIONS------------------------------------------------------*/

        public string ToString(int player)
        {
            string s = "";
            int q = player > 0 ? 1 : 0;

            // pieces and side border
            for (int i = 7 * q; i >= 0 && i < 8; i -= player)
            {
                s += $"\n\t{i + 1} |  "; // label 123

                for (int j = 7 * q; j >= 0 && j < 8; j -= player)
                {
                    char c = value[i, j] == 0
                            ? (i & 1) == (j & 1) ? ':' : '~'
                            : "kqrbnp PNBRQK"[6 + value[i, j]];

                    s += $" {c} ";
                }
            }

            // bottom border
            s += "\n\n\t      ";
            for (int i = 0; i < 7 * 3 + 1; i++) s += "-";

            // label abc
            s += "\n\n\t      ";
            for (int i = 7 * q; i < 8 && i >= 0; i -= player) s += $"{"hgfedcba"[i]}  ";

            s += "\n";

            return s;
        }
    }
}

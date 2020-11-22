using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_Chess_II
{
    class Board
    {
        /*--------------------------------------------------------ATTRIBUTES-*/

        // value of piece present in each square
        private int[,] value; 
        public int[,] Value { get { return value; } }

        // number of moves since square has been interacted with
        private uint[,] touch; 
        public uint[,] Touch { get { return touch; } }


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
            int p = value[move.src.y, move.src.x];

            // special cases
            switch ((uint)Math.Abs(p))
            {
                case Pieces.KING:

                    int dy = move.dst.y - move.src.y;
                    int dx = move.dst.x - move.src.x;

                    if (dy == 0 // if castling
                    && Math.Abs(dx) == 2)
                    {
                        // move rook
                        value[move.src.y, (int)(4 + (3.5f * Math.Sign(dx)))] = 0;
                        value[move.src.y, move.src.x + Math.Sign(dx)] = (int)Pieces.ROOK * player;
                    }

                    break;

                case Pieces.PAWN:

                    // enable pawn promotions
                    if (move.dst.y == (int)(4 + (3.5 * player)))
                    {
                        value[move.src.y, move.src.x] = (int)Pieces.QUEEN * player;
                    }
                    break;
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

        public bool PosIsHitBy(Pos2 pos, uint[] mask, int player)
        {
            // iterate through each piece type
            foreach(KeyValuePair<uint, IPiece> vp in Pieces.dict)
            {
                if (mask != null && mask.Contains(vp.Key)) continue; // check if piece is included

                // get all captures possible of piece from pos
                List<Pos2> captures = vp.Value.ListMoves(this, pos, player).c;

                // check all potential captures here
                foreach (Pos2 dst in captures)
                {
                    // if this is the right piece to attack pos
                    if (Math.Abs(value[dst.y, dst.x]) == vp.Key) return true;
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

                    if (Math.Sign(p) == player)
                    {
                        List<Pos2>[] sqrs = new List<Pos2>[2]; // destination squares
                        Pos2 src = new Pos2(i, j); // current piece's starting position

                        // get all pseudo-legal moves for current piece
                        (sqrs[0], sqrs[1]) = 
                            Pieces.dict[(uint)Math.Abs(p)].ListMoves(this, src, player);

                        foreach (List<Pos2> list in sqrs) // for each move type
                        {
                            foreach (Pos2 dst in list) // for each destination square
                            {
                                Move move = new Move(src, dst);

                                // if move doesn't result in check,
                                if (!MoveIsSuicide(move, player)) return false;
                            }
                        }
                    }
                }
            }
            // if you've checked every move and none worked...
            return true;
        }

        public List<Pos2> FindPiece(uint p, int player)
        {
            List<Pos2> found = new List<Pos2> { };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (value[i, j] == Math.Abs(p) * player)
                    {
                        found.Add(new Pos2(i, j));
                    }
                }
            }

            return found;
        }


        /*-MOVE LEGALITY EVALUATION FUNCTIONS--------------------------------*/

        public bool MoveIsPseudoLegal(Move move, int player)
        {
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
            foreach (Pos2 tmp in list)
                if (tmp.Equals(move.dst)) return true;

            return false;
        }

        public bool MoveIsSuicide(Move move, int player)
        {
            move ??= new Move(new Pos2(0, 0), new Pos2(0, 0));

            // copy board state
            Board test = new Board(this);
            // make move on new board
            test.MakeMove(move, player);

            // find kind in both variations
            Pos2[] pos = new Pos2[2];

            pos[0] = FindPiece(Pieces.KING, player)[0];
            pos[1] = test.FindPiece(Pieces.KING, player)[0];

            if (pos[0] == null || pos[1] == null) throw new Exception();

            do
            {
                // interpolate positions (if king is stationary do nothing)
                pos[0].y += Math.Sign(pos[1].y - pos[0].y);
                pos[0].x += Math.Sign(pos[1].x - pos[0].x);

                // if king is hit at any point during his journey
                if (test.PosIsHitBy(pos[0], null, player))
                {
                    return true;
                }
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

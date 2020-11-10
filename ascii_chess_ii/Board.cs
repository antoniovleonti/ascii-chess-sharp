using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_Chess_II
{
    delegate (List<Pos2> m, List<Pos2> c) ListMovesDel(Pos2 pos, int player);

    class Board
    {
        /*---------------------------------------------------------CONSTANTS-*/

        public const int PAWN = 1; // values of each piece on board
        public const int KNIGHT = 2;
        public const int BISHOP = 3;
        public const int ROOK = 4;
        public const int QUEEN = 5;
        public const int KING = 6;


        /*--------------------------------------------------------ATTRIBUTES-*/

        public int[,] piece; // piece present in each square
        public uint[,] touch; // time since square has been interacted with

        public ListMovesDel[] ListMoves; // array of piece movement functions


        /*-----------------------------------------------------------METHODS-*/


        /*-CONSTRUCTORS------------------------------------------------------*/

        public Board(int[,] _piece, uint[,] _touch) // new constructor
        {
            piece = new int[8, 8];
            touch = new uint[8, 8];

            Array.Copy(_piece, piece, 64);
            Array.Copy(_touch, touch, 64);

            ListMoves = InitListMoves();
        }

        public Board(Board other) // copy constructor
        {
            piece = new int[8, 8];
            touch = new uint[8, 8];

            Array.Copy(other.piece, piece, 64);
            Array.Copy(other.touch, touch, 64);

            ListMoves = InitListMoves();
        }

        // create array of ListXMoves functions (just for constructors)
        private ListMovesDel[] InitListMoves()
        {
            ListMovesDel[] ListMoves = new ListMovesDel[]
            {
                new ListMovesDel(ListPMoves),
                new ListMovesDel(ListNMoves),
                new ListMovesDel(ListBMoves),
                new ListMovesDel(ListRMoves),
                new ListMovesDel(ListQMoves),
                new ListMovesDel(ListKMoves),
            };

            return ListMoves;
        }


        /*-PIECE MOVEMENT FUNCTIONS------------------------------------------*/

        // helper function
        // go as far as possible in direction pointed to by vector {dy,dx}
        private (List<Pos2> m, List<Pos2> c)
            CastMoves(Pos2 pos, int dx, int dy, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int y, x;
            for (int i = 1; true; i++)
            {
                y = pos.y + dy * i;
                x = pos.x + dx * i;

                if (y >= 0 && y < 8 // within bounds
                && x >= 0 && x < 8 // ^^^
                && Math.Sign(piece[y, x]) != player) // or is not friendly piece
                {
                    if (Math.Sign(piece[y, x]) == -player)
                    {
                        captures.Add(new Pos2(y, x));
                        return (maneuvers, captures);
                    }
                    else maneuvers.Add(new Pos2(y, x));
                }
                else return (maneuvers, captures);
            }
        }

        // the following functions return 2 lists, 1 for maneuvers & 1 for captures.

        // pawn 
        public (List<Pos2> m, List<Pos2> c) ListPMoves(Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            // first determine maneuvers available to the pawn
            int y = pos.y + player;
            int x = pos.x;

            if (y < 8 && y >= 0
            && Math.Sign(piece[y, x]) == 0) // or is empty
            {
                // it can move forward 1
                maneuvers.Add(new Pos2(y, x));

                // if pawn is at home & nothing is 2 squares ahead
                y += player;
                if (touch[pos.y, x] == 0
                && Math.Sign(piece[y, x]) == 0) // or is empty
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
                && (Math.Sign(piece[y, x]) == -player // other player's piece                              
                    || (pos.y == (int)(3.5f + player) // or is en passant
                        && touch[pos.y, x] == 1
                        && touch[pos.y + player * 2, x] == 1)))
                {
                    captures.Add(new Pos2(y, x));
                }
            }
            return (maneuvers, captures);
        }

        // knight 
        public (List<Pos2> m, List<Pos2> c) ListNMoves(Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int[,] dydx =
            {
                { 2,-1},{ 2, 1},{-2,-1},{-2, 1},
                { 1,-2},{ 1, 2},{-1,-2},{-1, 2}
            };

            int y, x;
            for (int i = 0; i < (dydx.Length / dydx.Rank); i++)
            {
                y = pos.y + dydx[i, 0];
                x = pos.x + dydx[i, 1];


                if (y >= 0 && y < 8 // within bounds
                && x >= 0 && x < 8 // ^^^
                && Math.Sign(piece[y, x]) != player)
                {
                    if (Math.Sign(piece[y, x]) == -player)
                    {
                        captures.Add(new Pos2(y, x));
                    }
                    else maneuvers.Add(new Pos2(y, x));
                }
            }
            return (maneuvers, captures);
        }

        // bishop 
        public (List<Pos2> m, List<Pos2> c) ListBMoves(Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int dy, dx;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    dy = 1 - 2 * i; // every combination of 1 & -1
                    dx = 1 - 2 * j;

                    var (tmpm, tmpc) = CastMoves(pos, dy, dx, player);

                    maneuvers.AddRange(tmpm);
                    captures.AddRange(tmpc);
                }
            }
            return (maneuvers, captures);
        }

        // rook 
        public (List<Pos2> m, List<Pos2> c) ListRMoves(Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            int dy, dx;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    dy = i * (1 - 2 * j);
                    dx = (i == 0 ? 1 : 0) * (1 - 2 * j);

                    var (tmpm, tmpc) = CastMoves(pos, dy, dx, player);

                    maneuvers.AddRange(tmpm);
                    captures.AddRange(tmpc);
                }
            }
            return (maneuvers, captures);
        }

        // queen 
        public (List<Pos2> m, List<Pos2> c) ListQMoves(Pos2 pos, int player)
        {
            List<Pos2> maneuvers = new List<Pos2>();
            List<Pos2> captures = new List<Pos2>();

            for (int dx = -1; dx < 2; dx++)
            {
                for (int dy = -1; dy < 2; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    var (tmpm, tmpc) = CastMoves(pos, dy, dx, player);

                    maneuvers.AddRange(tmpm);
                    captures.AddRange(tmpc);
                }
            }
            return (maneuvers, captures);
        }

        // king 
        public (List<Pos2> m, List<Pos2> c) ListKMoves(Pos2 pos, int player)
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
                    && Math.Sign(piece[y, x]) != player) // or is not friendly piece
                    {
                        if (Math.Sign(piece[y, x]) == -player)
                        {
                            captures.Add(new Pos2(y, x));
                        }
                        else maneuvers.Add(new Pos2(y, x));
                    }
                }
            }

            // now find castling moves
            // TODO: implement chess960 castling rules (more general)
            if (touch[pos.y, pos.x] == 0 // untouched king
            && !PosIsHitBy(pos, ~(1u << (KING - 1)), player)) // not in check
            {
                for (int i = 0; i < 2; i++)
                {
                    int dx = 1 - 2 * i; // -1 or 1

                    if (touch[pos.y, (int)(4 + 3.5f * dx)] == 0) // untouched rook
                    {
                        bool flag = false;

                        // make sure squares are open
                        for (int j = 3 + dx; j > 0 && j < 7; j += dx)
                        {
                            if (piece[pos.y, j] != 0)
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

        // plays move on the board & handles special cases such as castling
        public void MakeMove(Move move, int player)
        {
            int p = piece[move.src.y, move.src.x];

            // special cases
            switch (p)
            {
                case KING:

                    int dy = move.dst.y - move.src.y;
                    int dx = move.dst.x - move.src.x;

                    if (dy == 0 // if castling
                    && Math.Abs(dx) == 2)
                    {
                        // move rook
                        piece[move.src.y, (int)(4 + (3.5f * Math.Sign(dx)))] = 0;
                        piece[move.src.y, move.src.x + Math.Sign(dx)] = ROOK * player;
                    }

                    break;

                case PAWN:

                    // enable pawn promotions
                    if (move.dst.y == (int)(4 + (3.5 * player)))
                    {
                        piece[move.src.y, move.src.x] = QUEEN * player;
                    }
                    break;
            }

            // perform move
            piece[move.dst.y, move.dst.x] = piece[move.src.y, move.src.x];
            piece[move.src.y, move.src.x] = 0;

            // increment touch
            for(int i = 0; i < 8; i++)
                for(int j = 0; j < 8; j++)
                    if (touch[i, j] > 0) touch[i, j]++;

            // remember these squares were touched
            touch[move.dst.y, move.dst.x] = touch[move.src.y, move.src.x] = 1;
        }


        /*-BOARD STATE QUERY FUNCTIONS---------------------------------------*/

        public bool PosIsHitBy(Pos2 pos, uint mask, int player)
        {
            // iterate through each piece type
            for (int i = 0; i < ListMoves.Length; i++)
            {
                if ((mask >> i & 1) == 0) continue; // check if piece is included

                // get all captures possible of piece i from pos
                List<Pos2> captures = ListMoves[i](pos, player).c;

                // check all potential captures here
                for (int j = 0; j < captures.Count; j++)
                {
                    int y = captures[j].y;
                    int x = captures[j].x;

                    // if this is the right piece to attack pos
                    // (it must be the other player's since it's a capture)
                    if (Math.Abs(piece[y, x]) == (i + 1)) return true;
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
                    int p = piece[i, j];

                    if (Math.Sign(p) == player)
                    {
                        List<Pos2>[] sqrs = new List<Pos2>[2]; // destination squares
                        Pos2 src = new Pos2(i, j); // current piece's starting position

                        // get all pseudo-legal moves for current piece
                        (sqrs[0], sqrs[1]) = ListMoves[Math.Abs(p) - 1](src, player);

                        foreach (List<Pos2> list in sqrs) // for each move type
                        {
                            foreach (Pos2 dst in list) // for each destination square
                            {
                                Move move = new Move(src, dst, 0);

                                // if move doesn't result in check,
                                if (!MoveIsSuicide(move, player)) return false;
                            }
                        }
                    }
                }
            }
            // once you've checked all possible moves,
            return true;
        }

        public List<Pos2> FindPiece(int p, int player)
        {
            List<Pos2> found = new List<Pos2> { };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (piece[i, j] == Math.Abs(p) * player)
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
            int[] p = { piece[move.src.y, move.src.x],
                        piece[move.dst.y, move.dst.x] };

            // if you're trying to move the other player's pieces (or no piece)...
            if (Math.Sign(p[0]) != player) return false;

            // get all possible moves with piece
            var (maneuvers, captures) =
                ListMoves[Math.Abs(p[0]) - 1](move.src, player);

            // decide whether to check captures or maneuvers
            List<Pos2> list = (p[1] == 0) ? maneuvers : captures;

            // try to find desired square in list
            foreach (Pos2 tmp in list)
                if (tmp.Equals(move.dst))
                    return true;

            return false;
        }

        public bool MoveIsSuicide(Move move, int player)
        {
            move ??= new Move(new Pos2(0,0), new Pos2(0,0), 0);

            // copy board state
            Board test = new Board(this);
            // make move on new board
            test.MakeMove(move, player);

            // find kind in both variations
            Pos2[] pos = new Pos2[2];

            pos[0] = FindPiece(KING, player)[0];
            pos[1] = test.FindPiece(KING, player)[0];

            if (pos[0] == null || pos[1] == null) throw new Exception();

            do
            {
                // interpolate positions (if king is stationary do nothing)
                pos[0].y += Math.Sign(pos[1].y - pos[0].y);
                pos[0].x += Math.Sign(pos[1].x - pos[0].x);

                // if king is hit at any point during his journey
                if (test.PosIsHitBy(pos[0], UInt16.MaxValue, player))
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
                    char c = piece[i, j] == 0
                        ? (i & 1) == (j & 1) ? ':' : '~'
                        : "kqrbnp PNBRQK"[6 + piece[i, j]];

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

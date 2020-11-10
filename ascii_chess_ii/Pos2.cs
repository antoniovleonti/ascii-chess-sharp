using System;

namespace ASCII_Chess_II
{
    public class Pos2 : IEquatable<Pos2>
    {
        /*--------------------------------------------------------ATTRIBUTES-*/
        public int y;
        public int x;


        /*-----------------------------------------------------------METHODS-*/

        public Pos2(int _y, int _x) // new constructor
        {
            y = _y;
            x = _x;
        }

        public Pos2(Pos2 other) // copy constructor
        {
            x = other.x;
            y = other.y;
        }

        public bool Equals(Pos2 other)
        {
            return (y == other.y) && (x == other.x);
        }

        public override string ToString()
        {
            return $"{{{y}, {x}}}";
        }
    }
}

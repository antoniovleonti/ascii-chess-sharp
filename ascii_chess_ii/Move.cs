namespace ASCII_Chess_II
{
    public class Move
    {
        // attributes
        public Pos2 src;
        public Pos2 dst;

        // constructor
        public Move(Pos2 _src, Pos2 _dst)
        {
            src = new Pos2(_src);
            dst = new Pos2(_dst);
        }

        public override string ToString()
        {
            return $"{{{ src.ToString() }, { dst.ToString() }}}";
        }
    }
}
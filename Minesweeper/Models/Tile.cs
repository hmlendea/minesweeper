namespace Minesweeper.Models
{
    public class Tile : ModelBase
    {
        public override string Id => $"{X},{Y}";

        public int X { get; set; }

        public int Y { get; set; }

        public int DangerLevel { get; set; }

        public bool Flagged { get; set; }

        public bool Cleared { get; set; }

        public bool Mined { get; set; }

        public Tile()
        {
            X = 0;
            Y = 0;
            Flagged = false;
            Cleared = false;
            Mined = false;
        }
    }
}

namespace Minesweeper.Models
{
    public class Tile
    {
        public int Size { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public int DangerLevel { get; set; }

        public bool Flagged { get; set; }

        public bool Cleared { get; set; }

        public bool Mined { get; set; }

        public Tile()
        {
            Size = 48;
            PositionX = 0;
            PositionY = 0;

            Flagged = false;
            Cleared = false;
            Mined = false;
        }
    }
}
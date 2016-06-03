namespace Minesweeper.Game
{
    public class Tile
    {
        int size, posX, posY;
        int dangerLevel;
        bool isFlagged, isCleared, isMined;

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public int PositionX
        {
            get { return posX; }
            set { posX = value; }
        }

        public int PositionY
        {
            get { return posY; }
            set { posY = value; }
        }

        public int DangerLevel
        {
            get { return dangerLevel; }
            set { dangerLevel = value; }
        }

        public bool IsFlagged
        {
            get { return isFlagged; }
            set { isFlagged = value; }
        }

        public bool IsCleared
        {
            get { return isCleared; }
            set { isCleared = value; }
        }

        public bool IsMined
        {
            get { return isMined; }
            set { isMined = value; }
        }

        public Tile()
        {
            size = 48;
            posX = 0;
            posY = 0;

            isFlagged = false;
            isCleared = false;
            isMined = false;
        }
    }
}
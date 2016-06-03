using Minesweeper.Models;

namespace Minesweeper.Game
{
    public class GameEngine
    {
        int minesCount;
        int gameTime;
        bool isRunning, alive;

        public GameTable GameTable { get; private set; }

        public bool Completed
        {
            get
            {
                if (!alive)
                    return false;

                foreach (Tile tile in GameTable.Tiles)
                    if ((!tile.Flagged && !tile.Cleared) || (tile.Flagged && !tile.Mined))
                        return false;

                return true;
            }
        }

        public int GameTime
        {
            get { return gameTime; }
        }

        public int TableSize { get; private set; }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        public bool Alive
        {
            get { return alive; }
        }

        public int FlagsLimit { get; private set; }

        public int FlagsPlaced { get; private set; }

        public int FlagsRemaining { get { return FlagsLimit - FlagsPlaced; } }

        public GameEngine(int tableSize, int mines)
        {
            TableSize = tableSize;

            minesCount = mines;
            FlagsLimit = mines;

            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(TimerTick));
        }

        bool TimerTick()
        {
            if (isRunning)
                gameTime += 1;
            
            return true;
        }

        public void NewGame()
        {
            alive = true;
            FlagsPlaced = 0;

            GameTable = new GameTable(TableSize, minesCount);

            gameTime = 0;
            isRunning = true;
        }

        public void ClearTile(int x, int y)
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            if (GameTable.Tiles[x, y].Cleared || GameTable.Tiles[x, y].Flagged)
                return;

            if (GameTable.Tiles[x, y].Mined)
            {
                alive = false;
                isRunning = false;
                return;
            }

            GameTable.Tiles[x, y].Cleared = true;

            if (GameTable.Tiles[x, y].DangerLevel == 0 && !GameTable.Tiles[x, y].Mined)
                for (int dir = 0; dir < 8; dir++)
                {
                    int x2 = dx[dir] + x;
                    int y2 = dy[dir] + y;

                    if (x2 >= 0 && x2 < TableSize && y2 >= 0 && y2 < TableSize)
                    if (!GameTable.Tiles[x2, y2].Cleared && !GameTable.Tiles[x2, y2].Flagged)
                        ClearTile(x2, y2);
                }
        }

        public void FlagTile(int x, int y)
        {
            if (GameTable.Tiles[x, y].Cleared)
                return;

            // Flag it
            if (!GameTable.Tiles[x, y].Flagged)
            {
                if (FlagsPlaced < FlagsLimit)
                {
                    GameTable.Tiles[x, y].Flagged = true;
                    FlagsPlaced += 1;
                }
            }
            // Unflag it
            else
            {
                GameTable.Tiles[x, y].Flagged = false;
                FlagsPlaced -= 1;
            }
        }
    }
}

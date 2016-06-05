using System;

using Minesweeper.Models;
using Minesweeper.Repositories;

namespace Minesweeper.Controllers
{
    public class GameController
    {
        TileRepository tileRepository;

        /// <summary>
        /// Gets the game time.
        /// </summary>
        /// <value>The game time.</value>
        public int GameTime { get; private set; }

        /// <summary>
        /// Gets the size of the table.
        /// </summary>
        /// <value>The size of the table.</value>
        public int TableSize { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the player is alive.
        /// </summary>
        /// <value><c>true</c> if alive; otherwise, <c>false</c>.</value>
        public bool Alive { get; private set; }

        /// <summary>
        /// Gets the mines count.
        /// </summary>
        /// <value>The mines count.</value>
        public int MinesCount { get; private set; }

        /// <summary>
        /// Gets the flags limit.
        /// </summary>
        /// <value>The flags limit.</value>
        public int FlagsLimit { get; private set; }

        /// <summary>
        /// Gets the flags placed.
        /// </summary>
        /// <value>The flags placed.</value>
        public int FlagsPlaced { get; private set; }

        /// <summary>
        /// Gets the flags remaining.
        /// </summary>
        /// <value>The flags remaining.</value>
        public int FlagsRemaining { get { return FlagsLimit - FlagsPlaced; } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Minesweeper.Game.GameEngine"/> is completed.
        /// </summary>
        /// <value><c>true</c> if completed; otherwise, <c>false</c>.</value>
        public bool Completed
        {
            get
            {
                if (!Alive)
                    return false;

                foreach (Tile tile in tileRepository.GetAll())
                    if ((!tile.Flagged && !tile.Cleared) || (tile.Flagged && !tile.Mined))
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Minesweeper.Game.GameEngine"/> class.
        /// </summary>
        /// <param name="tableSize">Table size.</param>
        /// <param name="mines">Mines.</param>
        public GameController(int tableSize, int mines)
        {
            tileRepository = new TileRepository();
            TableSize = tableSize;

            MinesCount = mines;
            FlagsLimit = mines;

            GenerateTable();

            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(TimerTick));
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void NewGame()
        {
            Alive = true;
            FlagsPlaced = 0;

            GenerateTable();

            GameTime = 0;
            IsRunning = true;
        }

        /// <summary>
        /// Clears the tile.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void ClearTile(int x, int y)
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            Tile tile = tileRepository.Get(x, y);

            if (tile.Cleared || tile.Flagged)
                return;

            if (tile.Mined)
            {
                Alive = false;
                IsRunning = false;
                return;
            }

            tile.Cleared = true;

            if (tile.DangerLevel == 0)
                for (int dir = 0; dir < 8; dir++)
                {
                    int x2 = dx[dir] + x;
                    int y2 = dy[dir] + y;

                    Tile tile2 = tileRepository.Get(x2, y2);

                    if (x2 >= 0 && x2 < TableSize && y2 >= 0 && y2 < TableSize)
                    if (!tile2.Cleared && !tile2.Flagged)
                        ClearTile(x2, y2);
                }
        }

        /// <summary>
        /// Flags the tile.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void FlagTile(int x, int y)
        {
            Tile tile = tileRepository.Get(x, y);

            if (tile.Cleared)
                return;

            // Flag it
            if (!tile.Flagged)
            {
                if (FlagsPlaced < FlagsLimit)
                {
                    tile.Flagged = true;
                    FlagsPlaced += 1;
                }
            }
            // Unflag it
            else
            {
                tile.Flagged = false;
                FlagsPlaced -= 1;
            }
        }

        bool TimerTick()
        {
            if (IsRunning)
                GameTime += 1;
            
            return true;
        }

        public void Regenerate()
        {
            GenerateTable();
            InitializeTiles();
            GenerateMines();
            GenerateDangerLevels(); 
        }

        void GenerateTable()
        {
            InitializeTiles();
            GenerateMines();
            GenerateDangerLevels();
        }

        void InitializeTiles()
        {
            tileRepository.Clear();

            for (int x = 0; x < TableSize; x++)
                for (int y = 0; y < TableSize; y++)
                {
                    Tile tile = new Tile();
                    tile.X = x;
                    tile.Y = y;

                    tileRepository.Add(tile);
                } 
        }

        void GenerateMines()
        {
            Random rnd = new Random();

            for (int mine = 0; mine < MinesCount; mine++)
            {
                int x = rnd.Next(0, TableSize);
                int y = rnd.Next(0, TableSize);

                Tile tile = tileRepository.Get(x, y);

                if (!tile.Mined)
                    tile.Mined = true;
                else
                    mine -= 1;
            }
        }

        void GenerateDangerLevels()
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            for (int x = 0; x < TableSize; x++)
                for (int y = 0; y < TableSize; y++)
                {
                    Tile tile = tileRepository.Get(x, y);

                    if (tile.Mined)
                        for (int dir = 0; dir < 8; dir++)
                            if (dx[dir] + x >= 0 && dx[dir] + x < TableSize && dy[dir] + y >= 0 && dy[dir] + y < TableSize)
                            {
                                Tile tile2 = tileRepository.Get(x + dx[dir], y + dy[dir]);

                                if (!tile2.Mined)
                                    tile2.DangerLevel += 1;
                            }
                }
        }

        /// <summary>
        /// Get a tile based on it's location
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Tile GetTile(int x, int y)
        {
            return tileRepository.Get(x, y);
        }
    }
}

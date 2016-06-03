using System;
using Minesweeper.Models;

namespace Minesweeper.Game
{
    /// <summary>
    /// Game table.
    /// </summary>
    public class GameTable
    {
        /// <summary>
        /// Gets or sets the tiles.
        /// </summary>
        /// <value>The tiles.</value>
        public Tile[,] Tiles { get; set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; private set; }

        /// <summary>
        /// Gets the mines count.
        /// </summary>
        /// <value>The mines count.</value>
        public int MinesCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Minesweeper.Game.GameTable"/> class.
        /// </summary>
        /// <param name="tableSize">Table size.</param>
        /// <param name="mines">Mines.</param>
        public GameTable(int tableSize, int mines)
        {
            Size = tableSize;
            MinesCount = mines;

            InitializeTiles();
            GenerateMines();
            GenerateDangerLevels();
        }

        /// <summary>
        /// Regenerate the table.
        /// </summary>
        public void Regenerate()
        {
            Clear();
            InitializeTiles();
            GenerateMines();
            GenerateDangerLevels(); 
        }

        /// <summary>
        /// Clears the table.
        /// </summary>
        void Clear()
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    Tiles[i, j] = null;
            
            Tiles = null;
        }

        /// <summary>
        /// Initializes the tiles.
        /// </summary>
        void InitializeTiles()
        {
            Tiles = new Tile[Size, Size];

            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                {
                    Tiles[i, j] = new Tile();
                    Tiles[i, j].X = i;
                    Tiles[i, j].Y = j;
                } 
        }

        /// <summary>
        /// Generates the mines.
        /// </summary>
        void GenerateMines()
        {
            Random rnd = new Random();

            for (int mine = 0; mine < MinesCount; mine++)
            {
                int x = rnd.Next(0, Size);
                int y = rnd.Next(0, Size);

                if (!Tiles[x, y].Mined)
                    Tiles[x, y].Mined = true;
                else
                    mine -= 1;
            }
        }

        void GenerateDangerLevels()
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    if (Tiles[x, y].Mined)
                        for (int dir = 0; dir < 8; dir++)
                            if (dx[dir] + x >= 0 && dx[dir] + x < Size && dy[dir] + y >= 0 && dy[dir] + y < Size)
                            if (!Tiles[x + dx[dir], y + dy[dir]].Mined)
                                Tiles[x + dx[dir], y + dy[dir]].DangerLevel += 1;
        }
    }
}


using System;
using Minesweeper.Models;

namespace Minesweeper.Game
{
    public class GameTable
    {
        Tile[,] tiles;
        int tableSize, mines;

        public Tile[,] Tiles
        {
            get { return tiles; }
            set { tiles = value; }
        }

        public GameTable(int tableSize, int mines)
        {
            this.tableSize = tableSize;
            this.mines = mines;

            InitializeTiles();
            GenerateMines();
            GenerateDangerLevels();
        }

        public void Regenerate()
        {
            ClearTable();
            InitializeTiles();
            GenerateMines();
            GenerateDangerLevels(); 
        }

        void ClearTable()
        {
            for (int i = 0; i < tableSize; i++)
                for (int j = 0; j < tableSize; j++)
                    tiles[i, j] = null;
            tiles = null;
        }

        void InitializeTiles()
        {
            tiles = new Tile[tableSize, tableSize];

            for (int i = 0; i < tableSize; i++)
                for (int j = 0; j < tableSize; j++)
                {
                    tiles[i, j] = new Tile();
                    tiles[i, j].PositionX = i;
                    tiles[i, j].PositionY = j;
                } 
        }

        void GenerateMines()
        {
            Random rnd = new Random();

            for (int mine = 0; mine < mines; mine++)
            {
                int x = rnd.Next(0, tableSize);
                int y = rnd.Next(0, tableSize);

                if (!tiles[x, y].Mined)
                    tiles[x, y].Mined = true;
                else
                    mine -= 1;
            }
        }

        void GenerateDangerLevels()
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            for (int x = 0; x < tableSize; x++)
                for (int y = 0; y < tableSize; y++)
                    if (tiles[x, y].Mined)
                        for (int dir = 0; dir < 8; dir++)
                            if (dx[dir] + x >= 0 && dx[dir] + x < tableSize && dy[dir] + y >= 0 && dy[dir] + y < tableSize)
                            if (!tiles[x + dx[dir], y + dy[dir]].Mined)
                                tiles[x + dx[dir], y + dy[dir]].DangerLevel += 1;
        }
    }
}


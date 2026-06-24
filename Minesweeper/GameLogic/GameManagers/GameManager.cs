using System;
using System.Collections.Generic;

using Minesweeper.Models;
using Minesweeper.Repositories;
using Minesweeper.Settings;

namespace Minesweeper.GameLogic.GameManagers
{
    public class GameManager : IGameManager
    {
        readonly TileRepository tileRepository;

        Random random;
        double timerAccumulator;

        public int GameTime { get; private set; }

        public int TableSize { get; private set; }

        public bool IsRunning { get; private set; }

        public bool Alive { get; private set; }

        public int MinesCount { get; private set; }

        public int FlagsLimit { get; private set; }

        public int FlagsPlaced { get; private set; }

        public int FlagsRemaining => FlagsLimit - FlagsPlaced;

        public bool Completed
        {
            get
            {
                if (!Alive)
                {
                    return false;
                }

                foreach (Tile tile in tileRepository.GetAll())
                {
                    if ((!tile.Flagged && !tile.Cleared) || (tile.Flagged && !tile.Mined))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public GameManager()
        {
            tileRepository = new TileRepository();
            TableSize = GameDefines.TableSize;
            MinesCount = GameDefines.MinesCount;
            FlagsLimit = MinesCount;
        }

        public void LoadContent()
        {
            random = new Random();
            NewGame();
        }

        public void UnloadContent() { }

        public void Update(double elapsedMilliseconds)
        {
            if (!IsRunning)
            {
                return;
            }

            timerAccumulator += elapsedMilliseconds;

            while (timerAccumulator >= 1000)
            {
                timerAccumulator -= 1000;
                GameTime++;
            }

            if (Completed)
            {
                IsRunning = false;
            }
        }

        public Tile GetTile(int x, int y) => tileRepository.Get(x, y);

        public IEnumerable<Tile> GetAllTiles() => tileRepository.GetAll();

        public void ClearTile(int x, int y)
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            Tile tile = tileRepository.Get(x, y);

            if (tile.Cleared || tile.Flagged)
            {
                return;
            }

            if (tile.Mined)
            {
                Alive = false;
                IsRunning = false;
                return;
            }

            tile.Cleared = true;

            if (tile.DangerLevel == 0)
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    int x2 = dx[dir] + x;
                    int y2 = dy[dir] + y;

                    if (x2 >= 0 && x2 < TableSize && y2 >= 0 && y2 < TableSize)
                    {
                        Tile neighbour = tileRepository.Get(x2, y2);

                        if (!neighbour.Cleared && !neighbour.Flagged)
                        {
                            ClearTile(x2, y2);
                        }
                    }
                }
            }
        }

        public void FlagTile(int x, int y)
        {
            Tile tile = tileRepository.Get(x, y);

            if (tile.Cleared)
            {
                return;
            }

            if (!tile.Flagged)
            {
                if (FlagsPlaced < FlagsLimit)
                {
                    tile.Flagged = true;
                    FlagsPlaced++;
                }
            }
            else
            {
                tile.Flagged = false;
                FlagsPlaced--;
            }
        }

        public void NewGame()
        {
            GameTime = 0;
            timerAccumulator = 0;
            Alive = true;
            IsRunning = true;
            FlagsPlaced = 0;

            InitializeTiles();
            GenerateMines();
            GenerateDangerLevels();
        }

        void InitializeTiles()
        {
            tileRepository.Clear();

            for (int x = 0; x < TableSize; x++)
            {
                for (int y = 0; y < TableSize; y++)
                {
                    tileRepository.Add(new Tile { X = x, Y = y });
                }
            }
        }

        void GenerateMines()
        {
            for (int mine = 0; mine < MinesCount; mine++)
            {
                int x = random.Next(0, TableSize);
                int y = random.Next(0, TableSize);

                Tile tile = tileRepository.Get(x, y);

                if (!tile.Mined)
                {
                    tile.Mined = true;
                }
                else
                {
                    mine--;
                }
            }
        }

        void GenerateDangerLevels()
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            for (int x = 0; x < TableSize; x++)
            {
                for (int y = 0; y < TableSize; y++)
                {
                    Tile tile = tileRepository.Get(x, y);

                    if (tile.Mined)
                    {
                        for (int dir = 0; dir < 8; dir++)
                        {
                            int nx = x + dx[dir];
                            int ny = y + dy[dir];

                            if (nx >= 0 && nx < TableSize && ny >= 0 && ny < TableSize)
                            {
                                Tile neighbour = tileRepository.Get(nx, ny);

                                if (!neighbour.Mined)
                                {
                                    neighbour.DangerLevel++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

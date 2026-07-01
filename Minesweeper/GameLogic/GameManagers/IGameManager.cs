using System.Collections.Generic;

using Minesweeper.Models;

namespace Minesweeper.GameLogic.GameManagers
{
    public interface IGameManager : IGameLogicManager
    {
        int GameTime { get; }

        int TableSize { get; }

        bool IsRunning { get; }

        bool Alive { get; }

        int MinesCount { get; }

        int FlagsLimit { get; }

        int FlagsPlaced { get; }

        int FlagsRemaining { get; }

        bool Completed { get; }

        Tile GetTile(int x, int y);

        IEnumerable<Tile> GetAllTiles();

        void ClearTile(int x, int y);

        void FlagTile(int x, int y);

        void NewGame();
    }
}

namespace Minesweeper.Settings
{
    public static class GameDefines
    {
        public const int MapTileSize = 40;

        public const int TableSize = 16;

        public const int MinesCount = 24;

        public const int InfoBarHeight = 60;

        public const int BoardWidth = TableSize * MapTileSize;   // 640

        public const int BoardHeight = TableSize * MapTileSize;  // 640

        public const int WindowWidth = BoardWidth;               // 640

        public const int WindowHeight = InfoBarHeight + BoardHeight; // 700
    }
}

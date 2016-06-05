using Minesweeper.Models;

namespace Minesweeper.Repositories
{
    public class TileRepository : Repository<Tile>
    {
        /// <summary>
        /// Get a tile by it's location.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Tile Get(int x, int y)
        {
            return Entities.Find(tile => tile.X == x && tile.Y == y);
        }
    }
}


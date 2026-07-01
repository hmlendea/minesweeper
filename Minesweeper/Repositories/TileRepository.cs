using Minesweeper.Models;

namespace Minesweeper.Repositories
{
    public class TileRepository : Repository<Tile>
    {
        public Tile Get(int x, int y)
            => Entities.Find(tile => tile.X == x && tile.Y == y);
    }
}

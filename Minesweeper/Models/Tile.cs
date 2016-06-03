namespace Minesweeper.Models
{
    /// <summary>
    /// Tile.
    /// </summary>
    public class Tile : EntityBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public new string Id { get { return X + "," + Y; } }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        /// <value>The X coordinate.</value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        /// <value>The Y coordinate.</value>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the danger level.
        /// </summary>
        /// <value>The danger level.</value>
        public int DangerLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Minesweeper.Models.Tile"/> is flagged.
        /// </summary>
        /// <value><c>true</c> if flagged; otherwise, <c>false</c>.</value>
        public bool Flagged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Minesweeper.Models.Tile"/> is cleared.
        /// </summary>
        /// <value><c>true</c> if cleared; otherwise, <c>false</c>.</value>
        public bool Cleared { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Minesweeper.Models.Tile"/> is mined.
        /// </summary>
        /// <value><c>true</c> if mined; otherwise, <c>false</c>.</value>
        public bool Mined { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Minesweeper.Models.Tile"/> class.
        /// </summary>
        public Tile()
        {
            Size = 48;
            X = 0;
            Y = 0;

            Flagged = false;
            Cleared = false;
            Mined = false;
        }
    }
}
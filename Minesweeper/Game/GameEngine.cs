using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Minesweeper.Game
{
    public class GameEngine
    {
        GameTable gameTable;
        Color primaryColor, secondaryColor, backgroundColor;
        Bitmap bmpFlag = Gdk.Pixbuf.LoadFromResource("Minesweeper.Resources.flag.png").ToBitmap();
        Bitmap bmpMine = Gdk.Pixbuf.LoadFromResource("Minesweeper.Resources.mine.png").ToBitmap();

        int tableSize, tileSpacing;
        int flagsPlaced, flagsLimit, minesCount;
        int gameTime;
        bool isRunning, alive;

        Gdk.Window gdkWindowTable, gdkWindowInfoBar;

        Brush[] dangerBrushes =
            {
                Brushes.Black, Brushes.Green, Brushes.Orange, Brushes.Red, Brushes.DarkRed,
                Brushes.Purple, Brushes.DarkBlue, Brushes.Blue, Brushes.Cyan
            };

        public Gdk.Window GdkWindowTable
        {
            get { return gdkWindowTable; }
            set { gdkWindowTable = value; }
        }

        public Gdk.Window GdkWindowInfoBar
        {
            get { return gdkWindowInfoBar; }
            set { gdkWindowInfoBar = value; }
        }

        public bool Completed
        {
            get
            {
                if (!alive)
                    return false;

                foreach (Tile tile in gameTable.Tiles)
                    if ((!tile.IsFlagged && !tile.IsCleared) || (tile.IsFlagged && !tile.IsMined))
                        return false;

                return true;
            }
        }

        public int GameTime
        {
            get { return gameTime; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        public bool Alive
        {
            get { return alive; }
        }

        public int TableSize
        {
            get { return tableSize; }
        }

        public int TileSpacing
        {
            get { return tileSpacing; }
            set { tileSpacing = value; }
        }

        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }

        public Color SecondaryColor
        {
            get { return secondaryColor; }
            set { secondaryColor = value; }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        public GameEngine(int tableSize, int mines)
        {
            this.tableSize = tableSize;
            tileSpacing = 2;

            minesCount = mines;
            flagsLimit = mines;

            backgroundColor = Color.GhostWhite;
            primaryColor = Color.CornflowerBlue;
            secondaryColor = Color.White;

            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(TimerTick));
        }

        bool TimerTick()
        {
            if (isRunning)
            {
                gameTime += 1;
                DrawInfoBar();
            }
            
            return true;
        }

        public void NewGame()
        {
            alive = true;
            flagsPlaced = 0;

            gameTable = new GameTable(tableSize, minesCount);

            gameTime = 0;
            isRunning = true;
        }

        public void DrawTable()
        {
            int x, y;
            int width, height;
            int tileSize;
            Graphics gfx = Gtk.DotNet.Graphics.FromDrawable(gdkWindowTable);
            Rectangle recTable;
            Brush brBakcground = new SolidBrush(backgroundColor);
            Brush brActiveTile = new SolidBrush(primaryColor);
            Brush brClearedTile = new SolidBrush(secondaryColor);
            Font font;
            StringFormat strFormat = new StringFormat();

            gdkWindowTable.GetSize(out width, out height);
            tileSize = (width - tableSize * tileSpacing) / tableSize;
            recTable = new Rectangle(0, 0, width, height);

            font = new Font("Sans", (int)(tileSize * 0.5), FontStyle.Regular);
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;

            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            gfx.FillRectangle(brBakcground, recTable);

            for (y = 0; y < tableSize; y++)
                for (x = 0; x < tableSize; x++)
                {
                    recTable = new Rectangle(
                        tileSpacing / 2 + x * tileSize + x * tileSpacing,
                        tileSpacing / 2 + y * tileSize + y * tileSpacing,
                        tileSize, tileSize);

                    if (gameTable.Tiles[x, y].IsCleared)
                        gfx.FillRectangle(brClearedTile, recTable);
                    else
                        gfx.FillRectangle(brActiveTile, recTable);

                    if (gameTable.Tiles[x, y].IsCleared && gameTable.Tiles[x, y].DangerLevel > 0)
                        gfx.DrawString(
                            gameTable.Tiles[x, y].DangerLevel.ToString(), font,
                            dangerBrushes[gameTable.Tiles[x, y].DangerLevel], recTable, strFormat);

                    if (gameTable.Tiles[x, y].IsMined && !alive)
                        gfx.DrawImage(bmpMine, recTable);
                    
                    if (gameTable.Tiles[x, y].IsFlagged)
                        gfx.DrawImage(bmpFlag, recTable);
                }
            
            gfx.Dispose();
        }

        public void DrawInfoBar()
        {
            Graphics gfx = Gtk.DotNet.Graphics.FromDrawable(gdkWindowInfoBar);
            Brush brBackground = new SolidBrush(backgroundColor);
            Brush brForeground = new SolidBrush(primaryColor);

            int width, height, widthThird;
            gdkWindowInfoBar.GetSize(out width, out height);
            widthThird = width / 3;

            Rectangle recWhole = new Rectangle(0, 0, width, height);
            Rectangle recLeft = new Rectangle(0, 0, widthThird, height);
            Rectangle recMiddle = new Rectangle(widthThird, 0, widthThird, height);
            Rectangle recRight = new Rectangle(widthThird * 2, 0, widthThird, height);

            Font f = new Font("Sans", (int)(Math.Min(width, height) * 0.5), FontStyle.Regular);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;

            string face;

            gfx.FillRectangle(brBackground, recWhole);

            if (alive)
                face = ":)";
            else
                face = ":(";

            gfx.DrawString((flagsLimit - flagsPlaced).ToString(), f, brForeground, recLeft, strFormat);
            gfx.DrawString(face, f, brForeground, recMiddle, strFormat);
            gfx.DrawString(string.Format("{0:00}:{1:00}", (gameTime / 60) % 60, gameTime % 60), f, brForeground, recRight, strFormat);

            gfx.Dispose();
        }

        public void ClearTile(int x, int y)
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            if (gameTable.Tiles[x, y].IsCleared || gameTable.Tiles[x, y].IsFlagged)
                return;

            if (gameTable.Tiles[x, y].IsMined)
            {
                alive = false;
                isRunning = false;
                return;
            }

            gameTable.Tiles[x, y].IsCleared = true;

            if (gameTable.Tiles[x, y].DangerLevel == 0 && !gameTable.Tiles[x, y].IsMined)
                for (int dir = 0; dir < 8; dir++)
                {
                    int x2 = dx[dir] + x;
                    int y2 = dy[dir] + y;

                    if (x2 >= 0 && x2 < TableSize && y2 >= 0 && y2 < TableSize)
                    if (!gameTable.Tiles[x2, y2].IsCleared && !gameTable.Tiles[x2, y2].IsFlagged)
                        ClearTile(x2, y2);
                }
        }

        public void FlagTile(int x, int y)
        {
            if (gameTable.Tiles[x, y].IsCleared)
                return;

            // Flag it
            if (!gameTable.Tiles[x, y].IsFlagged)
            {
                if (flagsPlaced < flagsLimit)
                {
                    gameTable.Tiles[x, y].IsFlagged = true;
                    flagsPlaced += 1;
                }
            }
            // Unflag it
            else
            {
                gameTable.Tiles[x, y].IsFlagged = false;
                flagsPlaced -= 1;
            }
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Minesweeper.Models;
using Minesweeper.Controllers;

namespace Minesweeper.Views
{
    /// <summary>
    /// Game window.
    /// </summary>
    public partial class GameWindow : Gtk.Window
    {
        GameController gameEngine;
        Color primaryColor, secondaryColor, backgroundColor;
        Bitmap bmpFlag = Gdk.Pixbuf.LoadFromResource("Minesweeper.Resources.flag.png").ToBitmap();
        Bitmap bmpMine = Gdk.Pixbuf.LoadFromResource("Minesweeper.Resources.mine.png").ToBitmap();
        int tileSpacing;

        Brush[] dangerBrushes =
            {
                Brushes.Black, Brushes.Green, Brushes.Orange, Brushes.Red, Brushes.DarkRed,
                Brushes.Purple, Brushes.DarkBlue, Brushes.Blue, Brushes.Cyan
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="Minesweeper.Views.GameWindow"/> class.
        /// </summary>
        public GameWindow()
            : base(Gtk.WindowType.Toplevel)
        {
            Build();

            daTable.DoubleBuffered = false;

            tileSpacing = 2;

            backgroundColor = Color.GhostWhite;
            primaryColor = Color.CornflowerBlue;
            secondaryColor = Color.White;

            gameEngine = new GameController(16, 24);

            NewGame();

            daTable.ExposeEvent += delegate
            {
                DrawTable();
            };
            daInfoBar.ExposeEvent += delegate
            {
                DrawInfoBar();
            };
            
            daTable.AddEvents((int)Gdk.EventMask.ButtonPressMask);
        }

        void NewGame()
        {
            gameEngine.NewGame();
            Draw();
        }

        void Draw()
        {
            DrawTable();
            DrawInfoBar();
        }

        static void ShowDialog(string title, string message, Gtk.MessageType msgType, Gtk.ButtonsType btnType)
        {
            Gtk.MessageDialog md = new Gtk.MessageDialog(null, Gtk.DialogFlags.Modal, msgType, btnType, message);
            md.Title = title;
            md.Run();
            md.Destroy();
        }

        void DrawTable()
        {
            int x, y;
            int width, height;
            int tileSize;
            Gdk.Drawable drawable = daTable.GdkWindow;
            Graphics gfx = Gtk.DotNet.Graphics.FromDrawable(drawable);
            Rectangle recTable;
            Brush brBakcground = new SolidBrush(backgroundColor);
            Brush brActiveTile = new SolidBrush(primaryColor);
            Brush brClearedTile = new SolidBrush(secondaryColor);
            Font font;
            StringFormat strFormat = new StringFormat();

            drawable.GetSize(out width, out height);
            tileSize = (width - gameEngine.TableSize * tileSpacing) / gameEngine.TableSize;
            recTable = new Rectangle(0, 0, width, height);

            font = new Font("Sans", (int)(tileSize * 0.5), FontStyle.Regular);
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;

            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            gfx.FillRectangle(brBakcground, recTable);

            for (y = 0; y < gameEngine.TableSize; y++)
                for (x = 0; x < gameEngine.TableSize; x++)
                {
                    recTable = new Rectangle(
                        tileSpacing / 2 + x * tileSize + x * tileSpacing,
                        tileSpacing / 2 + y * tileSize + y * tileSpacing,
                        tileSize, tileSize);

                    Tile tile = gameEngine.GetTile(x, y);

                    if (tile.Cleared)
                        gfx.FillRectangle(brClearedTile, recTable);
                    else
                        gfx.FillRectangle(brActiveTile, recTable);

                    if (tile.Cleared && tile.DangerLevel > 0)
                        gfx.DrawString(
                            tile.DangerLevel.ToString(), font,
                            dangerBrushes[tile.DangerLevel], recTable, strFormat);

                    if (tile.Mined && !gameEngine.Alive)
                        gfx.DrawImage(bmpMine, recTable);

                    if (tile.Flagged)
                        gfx.DrawImage(bmpFlag, recTable);
                }

            gfx.Dispose();
        }

        void DrawInfoBar()
        {
            Gdk.Drawable drawable = daInfoBar.GdkWindow;
            Graphics gfx = Gtk.DotNet.Graphics.FromDrawable(drawable);
            Brush brBackground = new SolidBrush(backgroundColor);
            Brush brForeground = new SolidBrush(primaryColor);


            int width, height, widthThird;
            drawable.GetSize(out width, out height);
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

            if (gameEngine.Alive)
                face = ":)";
            else
                face = ":(";

            gfx.DrawString(gameEngine.FlagsRemaining.ToString(), f, brForeground, recLeft, strFormat);
            gfx.DrawString(face, f, brForeground, recMiddle, strFormat);
            gfx.DrawString(string.Format("{0:00}:{1:00}", (gameEngine.GameTime / 60) % 60, gameEngine.GameTime % 60),
                f, brForeground, recRight, strFormat);

            gfx.Dispose();
        }

        /// <summary>
        /// Raises the da table button press event event.
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="args">Arguments.</param>
        protected void OnDaTableButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
        {
            if (gameEngine.IsRunning)
            {
                int tileSize = daTable.GdkWindow.GetWidth() / gameEngine.TableSize;
                int x = (int)args.Event.X / tileSize;
                int y = (int)args.Event.Y / tileSize;

                if (args.Event.Button == 1) // Left mouse click
                    gameEngine.ClearTile(x, y);
                else if (args.Event.Button == 3) // Right mouse click
                    gameEngine.FlagTile(x, y);

                Draw();

                if (gameEngine.Completed)
                {
                    ShowDialog("Level complete", "You have successfully cleared this level!", Gtk.MessageType.Info, Gtk.ButtonsType.Ok);

                    gameEngine.NewGame();
                    Draw();
                }
                else if (!gameEngine.Alive)
                {
                    ShowDialog("Game over", "You have detonated a bomb and lost this level! :(", Gtk.MessageType.Warning, Gtk.ButtonsType.Ok);

                    gameEngine.NewGame();
                    Draw();
                }
            }
        }

        /// <summary>
        /// Raises the retry action activated event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnRetryActionActivated(object sender, EventArgs e)
        {
            NewGame();
        }
    }
}


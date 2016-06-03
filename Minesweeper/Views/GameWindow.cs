using System;
using Gtk;
using Minesweeper.Game;

namespace Minesweeper.Views
{
    public partial class GameWindow : Window
    {
        GameEngine gameEngine;

        public GameWindow()
            : base(WindowType.Toplevel)
        {
            Build();
            daTable.DoubleBuffered = false;

            gameEngine = new GameEngine(16, 24);
            gameEngine.GdkWindowTable = daTable.GdkWindow;
            gameEngine.GdkWindowInfoBar = daInfoBar.GdkWindow;

            NewGame();

            daTable.ExposeEvent += delegate
            {
                gameEngine.DrawTable();
            };
            daInfoBar.ExposeEvent += delegate
            {
                gameEngine.DrawInfoBar();
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
            gameEngine.DrawTable();
            gameEngine.DrawInfoBar();
        }

        static void ShowDialog(string title, string message, MessageType msgType, ButtonsType btnType)
        {
            MessageDialog md = new MessageDialog(null, DialogFlags.Modal, msgType, btnType, message);
            md.Title = title;
            md.Run();
            md.Destroy();
        }

        protected void OnDaTableButtonPressEvent(object o, ButtonPressEventArgs args)
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
                    ShowDialog("Level complete", "You have successfully cleared this level!", MessageType.Info, ButtonsType.Ok);

                    gameEngine.NewGame();
                    Draw();
                }
                else if (!gameEngine.Alive)
                {
                    ShowDialog("Game over", "You have detonated a bomb and lost this level! :(", MessageType.Warning, ButtonsType.Ok);

                    gameEngine.NewGame();
                    Draw();
                }
            }
        }

        protected void OnRetryActionActivated(object sender, EventArgs e)
        {
            NewGame();
        }
    }
}


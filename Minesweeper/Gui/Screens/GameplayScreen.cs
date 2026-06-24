using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using NuciXNA.Gui;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;
using NuciXNA.Primitives;

using Minesweeper.GameLogic.GameManagers;
using Minesweeper.Gui.Controls;
using Minesweeper.Settings;

namespace Minesweeper.Gui.Screens
{
    public class GameplayScreen : Screen
    {
        IGameManager game;

        GuiInfoBar infoBar;
        GuiGameBoard gameBoard;

        double transitionDelay = -1;
        bool transitionScheduled;

        public GameplayScreen()
        {
            BackgroundColour = Colour.Black;
            ForegroundColour = Colour.White;
        }

        protected override void DoLoadContent()
        {
            game = new GameManager();
            game.LoadContent();

            infoBar = new GuiInfoBar(game);
            gameBoard = new GuiGameBoard(game)
            {
                Size = new Size2D(
                    GameDefines.TableSize * GameDefines.MapTileSize,
                    GameDefines.TableSize * GameDefines.MapTileSize)
            };

            GuiManager.Instance.RegisterControls(infoBar, gameBoard);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent()
        {
            game.UnloadContent();
            UnregisterEvents();
            SettingsManager.Instance.SaveContent();
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            game.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            if (transitionScheduled)
            {
                transitionDelay -= gameTime.ElapsedGameTime.TotalSeconds;

                if (transitionDelay <= 0)
                {
                    if (!game.Alive)
                    {
                        ScreenManager.Instance.ChangeScreens<GameOverScreen>();
                    }
                    else
                    {
                        ScreenManager.Instance.ChangeScreens<VictoryScreen>();
                    }

                    transitionScheduled = false;
                }
            }
            else if (!game.IsRunning && !transitionScheduled)
            {
                transitionScheduled = true;
                transitionDelay = game.Alive ? 1.5 : 2.0;
            }

            SetChildrenProperties();
        }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        void RegisterEvents()
        {
            InputManager.Instance.KeyboardKeyPressed += OnKeyboardKeyPressed;
            InputManager.Instance.MouseButtonPressed += OnMouseButtonPressed;
        }

        void UnregisterEvents()
        {
            InputManager.Instance.KeyboardKeyPressed -= OnKeyboardKeyPressed;
            InputManager.Instance.MouseButtonPressed -= OnMouseButtonPressed;
        }

        void SetChildrenProperties()
        {
            infoBar.Location = Point2D.Empty;
            infoBar.Size = new Size2D(GameDefines.BoardWidth, GameDefines.InfoBarHeight);
            infoBar.BackgroundColour = BackgroundColour;
            infoBar.ForegroundColour = ForegroundColour;

            gameBoard.Location = new Point2D(0, GameDefines.InfoBarHeight);
        }

        void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (!game.IsRunning)
            {
                return;
            }

            int relX = e.Location.X;
            int relY = e.Location.Y - GameDefines.InfoBarHeight;

            if (relX < 0 || relX >= GameDefines.BoardWidth || relY < 0 || relY >= GameDefines.BoardHeight)
            {
                return;
            }

            int tileX = relX / GameDefines.MapTileSize;
            int tileY = relY / GameDefines.MapTileSize;

            if (e.Button == MouseButton.Left)
            {
                game.ClearTile(tileX, tileY);
            }
            else if (e.Button == MouseButton.Right)
            {
                game.FlagTile(tileX, tileY);
            }
        }

        void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.N || e.Key == Keys.R)
            {
                RestartGame();
            }
            else if (e.Key == Keys.Escape)
            {
                ScreenManager.Instance.ChangeScreens<TitleScreen>();
            }
        }

        void RestartGame()
        {
            transitionScheduled = false;
            transitionDelay = -1;
            game.NewGame();
        }
    }
}

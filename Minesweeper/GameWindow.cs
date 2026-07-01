using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics;
using NuciXNA.Gui;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;

using Minesweeper.Gui;
using Minesweeper.Gui.Screens;
using Minesweeper.Settings;

namespace Minesweeper
{
    public class GameWindow : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        readonly FpsIndicator fpsIndicator;

        public GameWindow()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
                PreferredBackBufferWidth = GameDefines.WindowWidth,
                PreferredBackBufferHeight = GameDefines.WindowHeight
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            fpsIndicator = new FpsIndicator();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GraphicsManager.Instance.SpriteBatch = spriteBatch;
            GraphicsManager.Instance.Graphics = graphics;

            NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);
            SettingsManager.Instance.LoadContent();

            ScreenManager.Instance.SpriteBatch = spriteBatch;
            ScreenManager.Instance.StartingScreenType = typeof(SplashScreen);
            ScreenManager.Instance.LoadContent();

            fpsIndicator.LoadContent();
        }

        protected override void UnloadContent()
        {
            ScreenManager.Instance.UnloadContent();
            FpsIndicator.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            SettingsManager.Instance.Update();
            ScreenManager.Instance.Update(gameTime);

            if (IsActive)
            {
                InputManager.Instance.Update(Window);
            }
            else
            {
                InputManager.Instance.ResetInputStates();
            }

            fpsIndicator.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            ScreenManager.Instance.Draw(spriteBatch);

            fpsIndicator.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

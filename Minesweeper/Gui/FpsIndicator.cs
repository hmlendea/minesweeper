using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;

using Minesweeper.Gui.Helpers;
using Minesweeper.Settings;

namespace Minesweeper.Gui
{
    public class FpsIndicator
    {
        GameTime gameTime;
        SpriteFont fpsFont;
        string fpsString;

        public Vector2 Location { get; set; }

        public FpsIndicator() => Location = Vector2.Zero;

        public void LoadContent() => fpsFont = NuciContentManager.Instance.LoadSpriteFont("Fonts/FrameCounterFont");

        public static void UnloadContent() { }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            fpsString = $"FPS: {Math.Round(FramerateCounter.Instance.AverageFramesPerSecond)}";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FramerateCounter.Instance.Update(deltaTime);

            if (SettingsManager.Instance.DebugMode)
            {
                spriteBatch.DrawString(fpsFont, fpsString, Vector2.One, Color.Lime);
            }
        }
    }
}

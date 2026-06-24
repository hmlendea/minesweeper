using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using NuciXNA.Gui;
using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;
using NuciXNA.Primitives;

namespace Minesweeper.Gui.Screens
{
    public class VictoryScreen : Screen
    {
        float delay;
        GuiText messageText;

        public VictoryScreen()
        {
            delay = 4;
            BackgroundColour = Colour.Black;
            ForegroundColour = Colour.White;
        }

        protected override void DoLoadContent()
        {
            messageText = new GuiText
            {
                Text = "You Win!",
                FontName = "MenuFont",
                HorizontalAlignment = NuciXNA.Graphics.Drawing.Alignment.Middle,
                VerticalAlignment = NuciXNA.Graphics.Drawing.Alignment.Middle
            };

            GuiManager.Instance.RegisterControls(messageText);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime)
        {
            delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (delay <= 0)
            {
                ScreenManager.Instance.ChangeScreens<TitleScreen>();
            }

            SetChildrenProperties();
        }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        void RegisterEvents() => InputManager.Instance.KeyboardKeyPressed += OnKeyboardKeyPressed;

        void UnregisterEvents() => InputManager.Instance.KeyboardKeyPressed -= OnKeyboardKeyPressed;

        void SetChildrenProperties()
        {
            messageText.Location = Point2D.Empty;
            messageText.Size = new Size2D(
                ScreenManager.Instance.Size.Width,
                ScreenManager.Instance.Size.Height);
            messageText.BackgroundColour = BackgroundColour;
            messageText.ForegroundColour = new Colour(0, 200, 0);
        }

        void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.Enter || e.Key == Keys.Space || e.Key == Keys.Escape)
            {
                ScreenManager.Instance.ChangeScreens<TitleScreen>();
            }
        }
    }
}

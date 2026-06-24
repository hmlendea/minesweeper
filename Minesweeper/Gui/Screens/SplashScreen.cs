using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui;
using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;
using NuciXNA.Primitives;

namespace Minesweeper.Gui.Screens
{
    public class SplashScreen : Screen
    {
        public float Delay { get; set; }

        public GuiImage LogoImage { get; set; }

        public SplashScreen()
        {
            Delay = 2;
            BackgroundColour = Colour.Black;
        }

        protected override void DoLoadContent()
        {
            LogoImage = new GuiImage { ContentFile = "SplashScreen/Logo" };

            GuiManager.Instance.RegisterControls(LogoImage);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime)
        {
            if (Delay <= 0)
            {
                ChangeScreen();
            }

            Delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

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
            => LogoImage.Location = new Point2D(
                (ScreenManager.Instance.Size.Width - LogoImage.Size.Width) / 2,
                (ScreenManager.Instance.Size.Height - LogoImage.Size.Height) / 2);

        void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs e) => ChangeScreen();

        void OnMouseButtonPressed(object sender, MouseButtonEventArgs e) => ChangeScreen();

        static void ChangeScreen() => ScreenManager.Instance.ChangeScreens<TitleScreen>();
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using Minesweeper.GameLogic.GameManagers;

namespace Minesweeper.Gui.Controls
{
    public sealed class GuiInfoBar : GuiControl
    {
        readonly IGameManager game;

        GuiText flagsText;
        GuiText faceText;
        GuiText timerText;

        public GuiInfoBar(IGameManager game)
        {
            this.game = game;
            FontName = "InfoBarFont";
        }

        protected override void DoLoadContent()
        {
            flagsText = new GuiText
            {
                HorizontalAlignment = Alignment.Beginning,
                VerticalAlignment = Alignment.Middle
            };
            faceText = new GuiText
            {
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            };
            timerText = new GuiText
            {
                HorizontalAlignment = Alignment.End,
                VerticalAlignment = Alignment.Middle
            };

            RegisterChildren(flagsText, faceText, timerText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        void SetChildrenProperties()
        {
            int third = Size.Width / 3;

            flagsText.Location = Point2D.Empty;
            flagsText.Size = new Size2D(third, Size.Height);
            flagsText.BackgroundColour = BackgroundColour;
            flagsText.ForegroundColour = ForegroundColour;
            flagsText.Text = $"Mines: {game.FlagsRemaining}";

            faceText.Location = new Point2D(third, 0);
            faceText.Size = new Size2D(third, Size.Height);
            faceText.BackgroundColour = BackgroundColour;
            faceText.ForegroundColour = ForegroundColour;
            faceText.Text = game.Alive ? ":)" : ":(";

            timerText.Location = new Point2D(third * 2, 0);
            timerText.Size = new Size2D(third, Size.Height);
            timerText.BackgroundColour = BackgroundColour;
            timerText.ForegroundColour = ForegroundColour;
            timerText.Text = $"{game.GameTime / 60:D2}:{game.GameTime % 60:D2}";
        }
    }
}

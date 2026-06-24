using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Primitives;

namespace Minesweeper.Settings
{
    public class GraphicsSettings
    {
        public Size2D Resolution
        {
            get
            {
                if (Fullscreen)
                {
                    return new Size2D(
                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                }

                return new Size2D(GameDefines.WindowWidth, GameDefines.WindowHeight);
            }
        }

        public bool Fullscreen { get; set; }
    }
}

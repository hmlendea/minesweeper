using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using Minesweeper.GameLogic.GameManagers;
using Minesweeper.Models;
using Minesweeper.Settings;

namespace Minesweeper.Gui.Controls
{
    public class GuiGameBoard : GuiControl
    {
        static readonly Color[] DangerColors = new[]
        {
            Color.Black,   // 0 - unused
            Color.Blue,    // 1
            Color.Green,   // 2
            Color.Red,     // 3
            Color.Navy,    // 4
            Color.Maroon,  // 5
            Color.Purple,  // 6
            Color.Black,   // 7
            Color.Gray,    // 8
        };

        readonly IGameManager game;

        Texture2D pixelTexture;
        Texture2D mineTexture;
        Texture2D flagTexture;
        SpriteFont dangerFont;

        readonly Color unrevealedColor = new Color(160, 160, 160);
        readonly Color revealedColor = new Color(210, 210, 210);
        readonly Color gridLineColor = new Color(100, 100, 100);

        public GuiGameBoard(IGameManager game)
        {
            this.game = game;
        }

        protected override void DoLoadContent()
        {
            var graphicsDevice = GraphicsManager.Instance.Graphics.GraphicsDevice;

            pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            mineTexture = NuciContentManager.Instance.LoadTexture2D("Tiles/mine");
            flagTexture = NuciContentManager.Instance.LoadTexture2D("Tiles/flag");
            dangerFont = NuciContentManager.Instance.LoadSpriteFont("Fonts/InfoBarFont");
        }

        protected override void DoUnloadContent()
        {
            pixelTexture?.Dispose();
        }

        protected override void DoUpdate(GameTime gameTime) { }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            int tileSize = GameDefines.MapTileSize;
            int tableSize = game.TableSize;

            int originX = 0;
            int originY = GameDefines.InfoBarHeight;

            for (int y = 0; y < tableSize; y++)
            {
                for (int x = 0; x < tableSize; x++)
                {
                    Tile tile = game.GetTile(x, y);
                    int px = originX + x * tileSize;
                    int py = originY + y * tileSize;

                    Rectangle destRect = new Rectangle(px, py, tileSize - 1, tileSize - 1);

                    if (tile.Cleared)
                    {
                        spriteBatch.Draw(pixelTexture, destRect, revealedColor);

                        if (tile.DangerLevel > 0)
                        {
                            Color numColor = DangerColors[tile.DangerLevel];
                            string numText = tile.DangerLevel.ToString();
                            Vector2 textSize = dangerFont.MeasureString(numText);
                            Vector2 textPos = new Vector2(
                                px + (tileSize - textSize.X) / 2f,
                                py + (tileSize - textSize.Y) / 2f);

                            spriteBatch.DrawString(dangerFont, numText, textPos, numColor);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(pixelTexture, destRect, unrevealedColor);

                        if (tile.Flagged)
                        {
                            Rectangle flagRect = new Rectangle(
                                px + tileSize / 8,
                                py + tileSize / 8,
                                tileSize * 3 / 4,
                                tileSize * 3 / 4);
                            spriteBatch.Draw(flagTexture, flagRect, Color.White);
                        }
                        else if (tile.Mined && !game.Alive)
                        {
                            Rectangle mineRect = new Rectangle(
                                px + tileSize / 8,
                                py + tileSize / 8,
                                tileSize * 3 / 4,
                                tileSize * 3 / 4);
                            spriteBatch.Draw(mineTexture, mineRect, Color.White);
                        }
                    }
                }
            }
        }
    }
}

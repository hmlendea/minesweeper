using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using Minesweeper.GameLogic.GameManagers;
using Minesweeper.Models;
using Minesweeper.Settings;

namespace Minesweeper.Gui.Controls
{
    public class GuiGameBoard(IGameManager game) : GuiControl
    {
        static readonly Color[] DangerColors =
        [
            Color.Black,   // 0 - unused
            Color.Blue,    // 1
            Color.Green,   // 2
            Color.Red,     // 3
            Color.Navy,    // 4
            Color.Maroon,  // 5
            Color.Purple,  // 6
            Color.Black,   // 7
            Color.Gray,    // 8
        ];

        static readonly Rectangle2D UnrevealedSourceRect = new(0, 0, 512, 512);
        static readonly Rectangle2D ClearedSourceRect = new(512, 0, 512, 512);

        readonly IGameManager game = game;

        GuiImage[,] tileImages;
        Texture2D mineTexture;
        Texture2D flagTexture;
        SpriteFont dangerFont;

        protected override void DoLoadContent()
        {
            int tileSize = GameDefines.MapTileSize;
            int tableSize = game.TableSize;

            mineTexture = NuciContentManager.Instance.LoadTexture2D("Tiles/mine");
            flagTexture = NuciContentManager.Instance.LoadTexture2D("Tiles/flag");
            dangerFont = NuciContentManager.Instance.LoadSpriteFont("Fonts/InfoBarFont");

            tileImages = new GuiImage[tableSize, tableSize];
            List<GuiImage> imageList = new(tableSize * tableSize);

            for (int y = 0; y < tableSize; y++)
            {
                for (int x = 0; x < tableSize; x++)
                {
                    GuiImage image = new()
                    {
                        Id = $"{Id}_tile_{x}_{y}",
                        ContentFile = "Tiles/tiles",
                        Size = new Size2D(tileSize, tileSize),
                        Location = new Point2D(x * tileSize, y * tileSize),
                        SourceRectangle = UnrevealedSourceRect
                    };

                    tileImages[x, y] = image;
                    imageList.Add(image);
                }
            }

            RegisterChildren(imageList);
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime)
        {
            int tableSize = game.TableSize;

            for (int y = 0; y < tableSize; y++)
            {
                for (int x = 0; x < tableSize; x++)
                {
                    Tile tile = game.GetTile(x, y);
                    tileImages[x, y].SourceRectangle = tile.Cleared ? ClearedSourceRect : UnrevealedSourceRect;
                }
            }
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            int tileSize = GameDefines.MapTileSize;
            int tableSize = game.TableSize;

            for (int y = 0; y < tableSize; y++)
            {
                for (int x = 0; x < tableSize; x++)
                {
                    Tile tile = game.GetTile(x, y);
                    int px = x * tileSize;
                    int py = GameDefines.InfoBarHeight + y * tileSize;

                    if (tile.Cleared)
                    {
                        if (tile.DangerLevel > 0)
                        {
                            Color numColor = DangerColors[tile.DangerLevel];
                            string numText = tile.DangerLevel.ToString();
                            Vector2 textSize = dangerFont.MeasureString(numText);
                            Vector2 textPos = new(
                                px + (tileSize - textSize.X) / 2f,
                                py + (tileSize - textSize.Y) / 2f);

                            spriteBatch.DrawString(dangerFont, numText, textPos, numColor);
                        }
                    }
                    else
                    {
                        if (tile.Flagged)
                        {
                            Rectangle flagRect = new(
                                px + tileSize / 8,
                                py + tileSize / 8,
                                tileSize * 3 / 4,
                                tileSize * 3 / 4);
                            spriteBatch.Draw(flagTexture, flagRect, Color.White);
                        }
                        else if (tile.Mined && !game.Alive)
                        {
                            Rectangle mineRect = new(
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

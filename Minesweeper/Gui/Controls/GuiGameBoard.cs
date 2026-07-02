using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using Minesweeper.GameLogic.GameManagers;
using Minesweeper.Models;
using Minesweeper.Settings;

namespace Minesweeper.Gui.Controls
{
    public class GuiGameBoard(IGameManager game) : GuiControl
    {
        static readonly Colour[] DangerColors =
        [
            Colour.Black,   // 0 - unused
            Colour.Blue,    // 1
            Colour.Green,   // 2
            Colour.Red,     // 3
            Colour.Navy,    // 4
            Colour.Maroon,  // 5
            Colour.Purple,  // 6
            Colour.Black,   // 7
            Colour.Gray,    // 8
        ];

        static readonly Rectangle2D UnrevealedSourceRect = new(0, 0, 512, 512);
        static readonly Rectangle2D ClearedSourceRect = new(512, 0, 512, 512);

        readonly IGameManager game = game;

        GuiImage[,] tileImages;
        GuiImage[,] flagImages;
        GuiImage[,] mineImages;
        GuiText[,] dangerTexts;

        protected override void DoLoadContent()
        {
            int tileSize = GameDefines.MapTileSize;
            int tableSize = game.TableSize;
            int iconPad = tileSize / 8;
            int iconSize = tileSize * 3 / 4;

            tileImages = new GuiImage[tableSize, tableSize];
            flagImages = new GuiImage[tableSize, tableSize];
            mineImages = new GuiImage[tableSize, tableSize];
            dangerTexts = new GuiText[tableSize, tableSize];

            List<IGuiControl> children = new(tableSize * tableSize * 4);

            for (int y = 0; y < tableSize; y++)
            {
                for (int x = 0; x < tableSize; x++)
                {
                    tileImages[x, y] = new GuiImage
                    {
                        Id = $"{Id}_tile_{x}_{y}",
                        ContentFile = "Tiles/tiles",
                        Size = new Size2D(tileSize, tileSize),
                        Location = new Point2D(x * tileSize, y * tileSize),
                        SourceRectangle = UnrevealedSourceRect
                    };

                    flagImages[x, y] = new GuiImage
                    {
                        Id = $"{Id}_flag_{x}_{y}",
                        ContentFile = "Tiles/flag",
                        Size = new Size2D(iconSize, iconSize),
                        Location = new Point2D(x * tileSize + iconPad, y * tileSize + iconPad)
                    };

                    mineImages[x, y] = new GuiImage
                    {
                        Id = $"{Id}_mine_{x}_{y}",
                        ContentFile = "Tiles/mine",
                        Size = new Size2D(iconSize, iconSize),
                        Location = new Point2D(x * tileSize + iconPad, y * tileSize + iconPad)
                    };

                    dangerTexts[x, y] = new GuiText
                    {
                        Id = $"{Id}_danger_{x}_{y}",
                        FontName = "InfoBarFont",
                        Size = new Size2D(tileSize, tileSize),
                        Location = new Point2D(x * tileSize, y * tileSize),
                        HorizontalAlignment = Alignment.Middle,
                        VerticalAlignment = Alignment.Middle
                    };

                    children.Add(tileImages[x, y]);
                    children.Add(flagImages[x, y]);
                    children.Add(mineImages[x, y]);
                    children.Add(dangerTexts[x, y]);
                }
            }

            RegisterChildren(children);
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

                    SetVisibility(flagImages[x, y], !tile.Cleared && tile.Flagged);
                    SetVisibility(mineImages[x, y], !tile.Cleared && !tile.Flagged && tile.Mined && !game.Alive);

                    bool showDanger = tile.Cleared && tile.DangerLevel > 0;
                    SetVisibility(dangerTexts[x, y], showDanger);

                    if (showDanger)
                    {
                        dangerTexts[x, y].Text = tile.DangerLevel.ToString();
                        dangerTexts[x, y].ForegroundColour = DangerColors[tile.DangerLevel];
                    }
                }
            }
        }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        static void SetVisibility(IGuiControl control, bool visible)
        {
            if (visible)
            {
                control.Show();
            }
            else
            {
                control.Hide();
            }
        }
    }
}

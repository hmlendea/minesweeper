using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;

namespace Minesweeper.Gui.Screens
{
    public class TitleScreen : MenuScreen
    {
        GuiMenuLink newGameLink;
        GuiMenuLink settingsLink;

        protected override void DoLoadContent()
        {
            newGameLink = new GuiMenuLink
            {
                Id = nameof(newGameLink),
                Text = "New Game",
                TargetScreen = typeof(GameplayScreen)
            };
            settingsLink = new GuiMenuLink
            {
                Id = nameof(settingsLink),
                Text = "Settings",
                TargetScreen = typeof(SettingsScreen)
            };

            Items.Add(newGameLink);
            Items.Add(settingsLink);

            base.DoLoadContent();
        }
    }
}

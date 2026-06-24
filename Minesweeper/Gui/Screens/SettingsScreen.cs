using System;

using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;

using Minesweeper.Settings;

namespace Minesweeper.Gui.Screens
{
    public class SettingsScreen : MenuScreen
    {
        GuiMenuToggle fullScreenToggle;
        GuiMenuLink backLink;

        protected override void DoLoadContent()
        {
            fullScreenToggle = new GuiMenuToggle
            {
                Id = nameof(fullScreenToggle),
                Text = "Fullscreen"
            };
            backLink = new GuiMenuLink
            {
                Id = nameof(backLink),
                Text = "Back",
                TargetScreen = typeof(TitleScreen)
            };

            Items.Add(fullScreenToggle);
            Items.Add(backLink);

            RegisterEvents();

            fullScreenToggle.SetState(SettingsManager.Instance.GraphicsSettings.Fullscreen);

            base.DoLoadContent();
        }

        protected override void DoUnloadContent()
        {
            SettingsManager.Instance.SaveContent();
            UnregisterEvents();
            base.DoUnloadContent();
        }

        void RegisterEvents() => fullScreenToggle.StateChanged += OnFullscreenToggleStateChanged;

        void UnregisterEvents() => fullScreenToggle.StateChanged -= OnFullscreenToggleStateChanged;

        void OnFullscreenToggleStateChanged(object sender, EventArgs e)
            => SettingsManager.Instance.GraphicsSettings.Fullscreen = fullScreenToggle.IsOn;
    }
}

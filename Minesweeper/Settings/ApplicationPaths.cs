using System;
using System.IO;
using System.Reflection;

namespace Minesweeper.Settings
{
    public static class ApplicationPaths
    {
        static readonly string rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string UserDataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Minesweeper");

        public static string LogsDirectory => Path.Combine(UserDataDirectory, "Logs");

        public static string SettingsFile => Path.Combine(UserDataDirectory, "Settings.xml");
    }
}

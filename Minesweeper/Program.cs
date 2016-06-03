using Gtk;
using Minesweeper.Views;

namespace Minesweeper
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();

            GameWindow win = new GameWindow();
            win.Show();

            Application.Run();
        }
    }
}

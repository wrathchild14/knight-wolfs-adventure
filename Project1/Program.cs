using System;

namespace Project1
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var game = new KWAGame();
            game.Run();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class BlackHole : VoidObject
    {
        public BlackHole()
        {
            Random rnd = new Random();
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
            Draw(Coordinates.x, Coordinates.y);
        }

        public BlackHole(int x, int y)
        {
            Coordinates.x = x;
            Coordinates.y = y;
            Draw(Coordinates.x, Coordinates.y);
        }

        protected void Draw(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(x, y);
            Console.Write("\u00D8");

        }
    }
}

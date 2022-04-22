using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class Wormhole : VoidObject
    {
        public Wormhole()
        {
            Random rnd = new Random();
            _x = rnd.Next(console_x);
            _y = rnd.Next(console_y);
            Draw(_x, _y);
        }

        protected void Draw(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(x, y);
            Console.Write("@");

        }
    }
}

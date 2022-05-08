using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class BlackHole : VoidObject
    {
        protected override char Symbol { get; } = '\u00D8';
        protected override ConsoleColor Color { get; } = ConsoleColor.Yellow;
        public BlackHole()
        {
            Random rnd = new Random();
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
        }

        public BlackHole(int x, int y)
        {
            Coordinates.x = x;
            Coordinates.y = y;
        }

        override public void Draw()
        {
            if (FractionDisplayMode == DisplayMode.Fractions)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else Console.ForegroundColor = Color;
            Console.SetCursorPosition(Coordinates.x, Coordinates.y);
            Console.Write(Symbol);
        }
    }
}

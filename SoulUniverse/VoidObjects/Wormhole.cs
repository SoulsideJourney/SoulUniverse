using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class Wormhole : VoidObject
    {
        protected override char Symbol { get; } = '@';
        protected override ConsoleColor Color { get; } = ConsoleColor.Gray;
        public Wormhole()
        {
            Random rnd = new();
            Coordinates.x = rnd.Next(Universe.UniverseX);
            Coordinates.y = rnd.Next(Universe.UniverseY);
        }

        public Wormhole(int x, int y)
        {
            Coordinates.x = x;
            Coordinates.y = y;
        }

        override public void Draw()
        {
            if (FractionDisplayMode == DisplayMode.Fractions)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else Console.ForegroundColor = Color;
            Console.SetCursorPosition(Coordinates.x, Coordinates.y);
            Console.Write(Symbol);
        }
    }
}

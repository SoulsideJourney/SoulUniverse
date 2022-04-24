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
            //Draw();
            //Draw(Coordinates.x, Coordinates.y);
        }

        public BlackHole(int x, int y)
        {
            Coordinates.x = x;
            Coordinates.y = y;
            //Draw();
            //Draw(Coordinates.x, Coordinates.y);
        }

        override public void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(Coordinates.x, Coordinates.y);
            Console.Write("\u00D8");
        }

        //protected void Draw(int x, int y)
        //{
        //    Console.ForegroundColor = ConsoleColor.Yellow;
        //    Console.SetCursorPosition(x, y);
        //    Console.Write("\u00D8");

        //}
    }
}

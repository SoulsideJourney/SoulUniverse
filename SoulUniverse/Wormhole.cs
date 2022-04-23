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
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
            Draw();
            //Draw(Coordinates.x, Coordinates.y);
        }

        public Wormhole(int x, int y)
        {
            Coordinates.x = x;
            Coordinates.y = y;
            Draw();
            //Draw(Coordinates.x, Coordinates.y);
        }

        protected void Draw()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(Coordinates.x, Coordinates.y);
            Console.Write("@");
        }

        //protected void Draw(int x, int y)
        //{
        //    Console.ForegroundColor = ConsoleColor.Gray;
        //    Console.SetCursorPosition(x, y);
        //    Console.Write("@");
        //}
    }
}

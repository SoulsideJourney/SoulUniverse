using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class Star : VoidObject
    {
        private starClass _starClass;
        public enum starClass
        {
            W, O, B, A, F, G, K, M, L
        }

        public Star()
        {
            Random rnd = new Random();
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
            _starClass = (starClass)Enum.GetValues(typeof(starClass)).GetValue(rnd.Next(Enum.GetValues(typeof(starClass)).Length-1));
            Draw(Coordinates.x, Coordinates.y, _starClass);

        }

        public Star(int x, int y)
        {
            Random rnd = new Random();
            Coordinates.x = x;
            Coordinates.y = y;
            _starClass = (starClass)Enum.GetValues(typeof(starClass)).GetValue(rnd.Next(Enum.GetValues(typeof(starClass)).Length-1));
            Draw(Coordinates.x, Coordinates.y, _starClass);
        }

        public Star(starClass starClass)
        {
            Random rnd = new Random();
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
            _starClass = starClass;
            Draw(Coordinates.x, Coordinates.y, starClass);
            
        }

        public Star(int x, int y, starClass starClass)
        {
            _starClass = starClass;
            Draw(x, y, starClass);
        }

        protected void Draw(int x, int y, starClass starClass)
        {
            switch (starClass)
            {
                case starClass.W:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case starClass.O:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case starClass.B:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case starClass.A:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case starClass.F:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case starClass.G:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case starClass.K:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case starClass.M:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case starClass.L:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }

            Console.SetCursorPosition(x, y);
            Console.Write("*");

        }
    }
}

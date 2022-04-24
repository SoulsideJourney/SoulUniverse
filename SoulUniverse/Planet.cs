using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulUniverse
{
    internal class Planet : StarSystemObject
    {
        public PlanetClass planetClass { get; protected set; }
        public enum PlanetClass
        {
            Barren, Continental, GasGiant, Ocean
        }

        public Planet(int distance)
        {
            Distance = distance;
            Random rnd = new Random();
            planetClass = (PlanetClass)Enum.GetValues(typeof(PlanetClass)).GetValue(rnd.Next(Enum.GetValues(typeof(PlanetClass)).Length));
            int absoluteX = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - rnd.Next((int)Math.Pow(Distance, 2))));
            int absoluteY = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - Math.Pow(absoluteX, 2)));
            Coordinates.x = absoluteX * (int)Math.Pow(-1, rnd.Next(2));
            Coordinates.y = absoluteY * (int)Math.Pow(-1, rnd.Next(2));
        }

        public override void Draw()
        {
            //Отрисовка орбиты
            Console.ResetColor();
            int absoluteX;
            int absoluteY;
            Random rnd = new Random();

            for (int i = 0; i <= Distance; i++)
            {
                //absoluteX = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - i));
                absoluteX = i;
                absoluteY = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - Math.Pow(absoluteX, 2)));
                Console.SetCursorPosition(20 + absoluteX, 20 + absoluteY);
                Console.Write("\u00B7");
                Console.SetCursorPosition(20 + absoluteX * -1, 20 + absoluteY);
                Console.Write("\u00B7");
                Console.SetCursorPosition(20 + absoluteX * -1, 20 + absoluteY * -1);
                Console.Write("\u00B7");
                Console.SetCursorPosition(20 + absoluteX, 20 + absoluteY * -1);
                Console.Write("\u00B7");
            }

            //Отрисовка звезды
            switch (planetClass)
            {
                case PlanetClass.Barren:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case PlanetClass.Continental:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case PlanetClass.GasGiant:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case PlanetClass.Ocean:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.SetCursorPosition(20 + Coordinates.x, 20 + Coordinates.y);
            Console.Write("*");
        }
    }
}

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
        }

        public override void Draw()
        {
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
            Random rnd = new Random();
            int x = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - rnd.Next((int)Math.Pow(Distance, 2))));
            int y = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - Math.Pow(x,  2)));
            Console.SetCursorPosition(20 + x, 20 + y);
            Console.Write("*");
        }
    }
}

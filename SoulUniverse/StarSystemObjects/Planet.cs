using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class Planet : StarSystemObject
    {
        protected override char Symbol { get; } = '\u25CF'; //●
        public override int Size { get; }
        public override double OrbitalSpeed { get; } = 107_000; //км*ч
        public PlanetClass PlanetClass { get; protected set; }

        public bool IsColonized { get; protected set; } = false;

        //List<KeyValuePair<Recource, int>> recources = new();
        private int universe_x;

        public Planet(int distance)
        {
            Distance = distance;
            Random rnd = new();
            PlanetClass = (PlanetClass)Enum.GetValues(typeof(PlanetClass)).GetValue(rnd.Next(Enum.GetValues(typeof(PlanetClass)).Length));
            int absoluteX = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - rnd.Next((int)Math.Pow(Distance, 2))));
            int absoluteY = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - Math.Pow(absoluteX, 2)));
            Coordinates.x = absoluteX * (int)Math.Pow(-1, rnd.Next(2));
            Coordinates.y = absoluteY * (int)Math.Pow(-1, rnd.Next(2));
            Size = rnd.Next(10000, 25000);
            foreach (Recource res in Enum.GetValues(typeof(Recource)))
            {
                recources.Add(new KeyValuePair<Recource, int>(res, rnd.Next(100000000)));
            }
        }

        public override void Draw()
        {
            ////Отрисовка орбиты
            //Console.ResetColor();
            //int absoluteX;
            //int absoluteY;

            //for (int i = 0; i <= Distance; i++)
            //{
            //    //absoluteX = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - i));
            //    absoluteX = i;
            //    absoluteY = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - Math.Pow(absoluteX, 2)));
            //    Console.SetCursorPosition(20 + absoluteX, 20 + absoluteY);
            //    Console.Write("\u00B7");
            //    Console.SetCursorPosition(20 + absoluteX * -1, 20 + absoluteY);
            //    Console.Write("\u00B7");
            //    Console.SetCursorPosition(20 + absoluteX * -1, 20 + absoluteY * -1);
            //    Console.Write("\u00B7");
            //    Console.SetCursorPosition(20 + absoluteX, 20 + absoluteY * -1);
            //    Console.Write("\u00B7");
            //}

            //Отрисовка планеты
            if (FractionDisplayMode == DisplayMode.Fractions)
            {
                if (Fractions.Count > 0) Console.ForegroundColor = Fractions.ElementAt(0).Color;
                else Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else
            {
                Console.ForegroundColor = GetColor();
            }
            Console.SetCursorPosition(20 + Coordinates.x, 20 + Coordinates.y);
            Console.Write(Symbol);
            DrawedCoordinates.x = Coordinates.x;
            DrawedCoordinates.y = Coordinates.y;
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
            isNeedToRedraw = false;
        }

        public void Erase()
        {
            Console.SetCursorPosition(20 + DrawedCoordinates.x, 20 + DrawedCoordinates.y);
            Console.Write(' ');
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
        }

        public void DrawMacro()
        {
            int center = 20;
            Console.ForegroundColor = this.GetColor();
            for (int i = center - 12; i <= center + 12; i++)
            {
                Console.SetCursorPosition(i, center);
                Console.Write('*');
            }
            Console.SetCursorPosition(center, center - 6);
            for (int i = center - 6; i <= center + 6; i++)
            {
                Console.SetCursorPosition(center, i);
                Console.Write('*');
            }
            Console.ResetColor();
        }

        public ConsoleColor GetColor()
        {
            switch (PlanetClass)
            {
                case PlanetClass.Barren:
                    return ConsoleColor.DarkYellow;
                case PlanetClass.Continental:
                    return ConsoleColor.Green;
                case PlanetClass.GasGiant:
                    return ConsoleColor.DarkBlue;
                case PlanetClass.Ocean:
                    return ConsoleColor.DarkBlue;
                default:
                    return ConsoleColor.White;
            }
        }

        public void Move()
        {
            //Угловая скорость
            double w = OrbitalSpeed / ((double)Distance * 150_000_000); // рад/ч
            double t = 24; //часов
            Phi = (Phi + w * t) % (2 * Math.PI);
            int newX = (int)Math.Round(((double)Distance * Math.Cos(Phi))); // а. е.
            int newY = (int)Math.Round(((double)Distance * Math.Sin(Phi))); // а. е.
            if (Coordinates.x != newX || Coordinates.y != newY)
            {
                Coordinates.x = newX;
                Coordinates.y = newY;
                isNeedToRedraw = true;
            }

        }
    }
}

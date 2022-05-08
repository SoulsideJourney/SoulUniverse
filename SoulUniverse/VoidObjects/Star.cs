using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class Star : VoidObject
    {
        protected override char Symbol { get; } = '*';
        protected override ConsoleColor Color { get; }

        public StarClass starClass;
        public List<StarSystemObject> starSystemObjects = new();

        public Star()
        {
            Random rnd = new();
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
            starClass = (StarClass)Enum.GetValues(typeof(StarClass)).GetValue(rnd.Next(Enum.GetValues(typeof(StarClass)).Length - 1));
            Color = GetColor(starClass);
            GenerateStarSystemObjects();

        }
        
        //public Star(int x, int y)
        //{
        //    Random rnd = new();
        //    Coordinates.x = x;
        //    Coordinates.y = y;
        //    starClass = (StarClass)Enum.GetValues(typeof(StarClass)).GetValue(rnd.Next(Enum.GetValues(typeof(StarClass)).Length - 1));
        //    Color = GetColor(starClass);
        //    GenerateStarSystemObjects();
        //}

        //public Star(StarClass starClass)
        //{
        //    Random rnd = new();
        //    Coordinates.x = rnd.Next(universe_x);
        //    Coordinates.y = rnd.Next(universe_y);
        //    this.starClass = starClass;
        //    GenerateStarSystemObjects();
        //}

        //public Star(int x, int y, StarClass starClass)
        //{
        //    this.starClass = starClass;
        //    GenerateStarSystemObjects();
        //}

        private void GenerateStarSystemObjects()
        {
            Random rnd = new();
            int objectCount = rnd.Next(15);
            //Генерация объектов системы
            for (int i = 0; i < objectCount; i++)
            {
                bool isPositionOccupied = false;
                while (!isPositionOccupied)
                {
                    int x = rnd.Next(2, universe_y / 2 - 1);
                    foreach (StarSystemObject obj in starSystemObjects)
                    {
                        if (obj.Distance == x)
                        {
                            isPositionOccupied = true;
                            break;
                        }
                    }
                    if (isPositionOccupied) continue;

                    //Позиция свободна -- добавляем планету
                    else
                    {
                        Planet planet = new(x);
                        //Добавляем фракцию на планету с небольшой долей вероятности
                        while (true)
                        {
                            if (rnd.Next(100) < 10)
                            {
                                planet.Fractions.Add(NPCFractions.ElementAt(rnd.Next(NPCFractions.Count)));
                            }
                            else break;
                        }
                        starSystemObjects.Add(planet);
                        break;
                    }
                }
            }
            starSystemObjects.Sort();
        }

        override public void Draw()
        {
            Draw(Coordinates.x, Coordinates.y, starClass);
        }
        public void Draw(int x, int y)
        {
            Draw(x, y, starClass);
        }

        protected void Draw(int x, int y, StarClass starClass)
        {
            if (FractionDisplayMode == DisplayMode.Fractions)
            {
                //Фракционный цвет звезды по цвету первой попавшейся фракции в системе окда?
                Console.ForegroundColor = starSystemObjects.Find(obj => obj.Fractions.Count > 0)?.Fractions.ElementAt(0).Color ?? ConsoleColor.DarkGray;
            }
            else
            {
                Console.ForegroundColor = GetColor();
            }
            Console.SetCursorPosition(x, y);
            Console.Write("*");
        }

        public void WriteStarInfo()
        {
            int row = 2;
            string starClass = this.starClass.ToString();
            Console.Write(string.Format("Звезда класса {0}", starClass));
            Console.SetCursorPosition(universe_x + 2, ++row);
            int planets = this.starSystemObjects.Count;
            Console.Write(string.Format("Количество планетарных тел: {0}", planets));

            //Информация о планетах
            if (planets > 0)
            {
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write("Информация об объектах системы:");
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write("------------------------------------------");
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write("| № | Расстояние до звезды | Класс       |");
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write("------------------------------------------");
                int planetNumber = 0;
                foreach (StarSystemObject starSystemObject in this.starSystemObjects)
                {
                    Console.SetCursorPosition(universe_x + 2, ++row);
                    string planetClass;
                    if (starSystemObject is Planet planet)
                    {
                        planetClass = planet.PlanetClass.ToString();

                    }
                    else planetClass = "";
                    Console.Write(string.Format("|{0, 2} | {1, 16} а.е | {2, 11} |", ++planetNumber, starSystemObject.Distance, planetClass));
                }
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write("------------------------------------------");

                //Информация о присутствующих фракциях
                Console.SetCursorPosition(universe_x + 2, ++row);
                List<Fraction> fractionsInSystem = new List<Fraction>();
                foreach (StarSystemObject starSystemObject in this.starSystemObjects)
                {
                    foreach (Fraction fraction in starSystemObject.Fractions)
                    {
                        if (!fractionsInSystem.Contains(fraction)) fractionsInSystem.Add(fraction);
                    }
                }
                if (fractionsInSystem.Count > 0)
                {
                    Console.Write("Присутствующие фракции:");
                    foreach (Fraction fraction in fractionsInSystem)
                    {
                        Console.SetCursorPosition(universe_x + 2, ++row);
                        Console.ForegroundColor = fraction.Color;
                        Console.Write(fraction.Name);
                    }
                    Console.ResetColor();
                }
                else Console.Write("Нет присутствующих фракций");
            }

            infoIsClear = false;

            //Возвращение курсора
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
        }

        //public void WriteSystemInfo(int row)
        //{
        //    Console.SetCursorPosition(universe_x + 2, ++row);
        //    Console.Write("Информация об объектах системы:");
        //    Console.SetCursorPosition(universe_x + 2, ++row);
        //    Console.Write("------------------------------------------");
        //    Console.SetCursorPosition(universe_x + 2, ++row);
        //    Console.Write("| № | Расстояние до звезды | Класс       |");
        //    Console.SetCursorPosition(universe_x + 2, ++row);
        //    Console.Write("------------------------------------------");
        //    int planetNumber = 0;
        //    foreach (StarSystemObject starSystemObject in this.starSystemObjects)
        //    {
        //        Console.SetCursorPosition(universe_x + 2, ++row);
        //        string planetClass;
        //        if (starSystemObject is Planet planet)
        //        {
        //            planetClass = planet.PlanetClass.ToString();

        //        }
        //        else planetClass = "";
        //        Console.Write(string.Format("|{0, 2} | {1, 16} а.е | {2, 11} |", ++planetNumber, starSystemObject.Distance, planetClass));
        //    }
        //    Console.SetCursorPosition(universe_x + 2, ++row);
        //    Console.Write("------------------------------------------");
        //}

        private ConsoleColor GetColor()
        {
            return GetColor(starClass);
        }
        
        private ConsoleColor GetColor(StarClass starClass)
        {
            switch (starClass)
            {
                case StarClass.W:
                    return ConsoleColor.Magenta;
                case StarClass.O:
                    return ConsoleColor.DarkBlue;
                case StarClass.B:
                    return ConsoleColor.Blue;
                case StarClass.A:
                    return ConsoleColor.DarkCyan;
                case StarClass.F:
                    return ConsoleColor.White;
                case StarClass.G:
                    return ConsoleColor.Yellow;
                case StarClass.K:
                    return ConsoleColor.DarkYellow;
                case StarClass.M:
                    return ConsoleColor.Red;
                case StarClass.L:
                    return ConsoleColor.DarkRed;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}

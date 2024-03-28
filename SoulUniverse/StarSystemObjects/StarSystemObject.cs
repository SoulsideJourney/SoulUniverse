using SoulUniverse.PlanetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    public abstract class StarSystemObject : UniverseObject, IComparable<StarSystemObject>, IMovable
    {
        protected abstract char Symbol { get; }
        public abstract int Size { get; init; }

        /// <summary>Орбитальная скорость в км/ч</summary>
        public abstract double OrbitalSpeed { get; }
        public bool IsNeedToRedraw { get; set; } = false;
        //blic bool isNeedToRedraw = false;
        public int Distance { get; init; }
        protected double Phi = 0;
        //public Coordinates DrawedCoordinates = new();
        public Coordinates DrawedCoordinates { get; set; } = new();
        public Coordinates Coordinates = new();
        public List<GroundObject> GroundObjects = new();
        public bool IsColonized => Fractions.Count > 0;
        public Dictionary<ResourceName, int> Recources { get; } = new();

        //Присутствующие фракции
        public List<Fraction> Fractions { get; } = new();

        public abstract ConsoleColor Color { get; }

        public StarSystemObject(int distance)
        {
            Distance = distance;
            Random rnd = new();
            Phi = rnd.NextDouble() * 2 * Math.PI;
            Coordinates.x = (int)Math.Round(Distance * Math.Cos(Phi));
            Coordinates.y = (int)Math.Round(Distance * Math.Sin(Phi));
        }

        public void DrawObjects()
        {
            mutex.WaitOne();
            foreach (var obj in GroundObjects)
            {
                obj.Draw();
            }
            mutex.ReleaseMutex();
        }

        public void WriteInfo()
        {
            lock (locker)
            {
                int row = 2;
                Console.ResetColor();

                //Общая информация
                Console.SetCursorPosition(universe_x + 2, row);
                Console.Write("Информация об объекте: ");
                Console.SetCursorPosition(universe_x + 2, ++row);
                if (this is Planet planet)
                {
                    //string planetClass = planet.PlanetClass.ToString();
                    Console.Write("Планета класса ");
                    Console.ForegroundColor = planet.Color;
                    Console.Write(planet.PlanetClass);
                    //Console.Write(string.Format("Планета класса {0}", planetClass));
                    Console.ResetColor();
                    Console.SetCursorPosition(universe_x + 2, ++row);
                    Console.Write(string.Format("Размер: {0} км", planet.Size));
                    Console.SetCursorPosition(universe_x + 2, ++row);
                    Console.Write(string.Format("Расстояние до родительского тела: {0} а.е. ", planet.Distance));
                    Console.SetCursorPosition(universe_x + 2, ++row);              
                }
                else Console.Write("Неизвестный объект");

                //Информация о ресурсах
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write("Имеющиеся ресурсы:");
                foreach (var resource in this.Recources)
                {
                    Console.SetCursorPosition(universe_x + 2, ++row);
                    Console.Write("{0}: {1}", resource.Key.ToString(), resource.Value.ToString());
                }
                infoIsClear = false;

                //Информация о фракциях
                if (Fractions.Count > 0)
                {
                    Console.SetCursorPosition(universe_x + 2, ++row);
                    Console.Write("Фракции:");
                    foreach (var fraction in Fractions)
                    {
                        Console.SetCursorPosition(universe_x + 2, ++row);
                        Console.ForegroundColor = fraction.Color;
                        Console.Write($"{fraction.Name} ({fraction.Property.Where( _ => _.Location == this).Count()} объектов)");
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.SetCursorPosition(universe_x + 2, ++row);
                    Console.Write("На планете нет фракций");
                }

                //Возвращение курсора
                Console.SetCursorPosition(current_cursor_x, current_cursor_y);
            }
        }

        public void Move()
        {
            //Угловая скорость
            double w = OrbitalSpeed / ((double)Distance * 150_000_000); // рад/ч
            double t = 24; //часов
            Phi = (Phi + w * t) % (2 * Math.PI);
            int newX = (int)Math.Round(Distance * Math.Cos(Phi)); // а. е.
            int newY = (int)Math.Round(Distance * Math.Sin(Phi)); // а. е.
            if (Coordinates.x != newX || Coordinates.y != newY)
            {
                Coordinates.x = newX;
                Coordinates.y = newY;
                IsNeedToRedraw = true;
            }
        }

        public override void Draw()
        {
            lock (locker)
            {
                //Отрисовка планеты
                if (FractionDisplayMode == DisplayMode.Fractions)
                {
                    if (IsColonized) Console.ForegroundColor = Fractions.ElementAt(0).Color;
                    else Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    Console.ForegroundColor = Color;
                }
                Console.SetCursorPosition(20 + Coordinates.x, 20 + Coordinates.y);
                Console.Write(Symbol);
                DrawedCoordinates.x = Coordinates.x;
                DrawedCoordinates.y = Coordinates.y;
                Console.SetCursorPosition(current_cursor_x, current_cursor_y);
                IsNeedToRedraw = false;
            }
        }

        public void Erase()
        {
            lock (locker)
            {
                Console.SetCursorPosition(20 + DrawedCoordinates.x, 20 + DrawedCoordinates.y);
                Console.Write(' ');
                Console.SetCursorPosition(current_cursor_x, current_cursor_y);
            }
        }

        /// <summary>Занято ли место на планете</summary>
        public bool IsPlaceOccupied(int x, int y)
        {
            if (GroundObjects.Any(o => o.Coordinates.x == x && o.Coordinates.y == y)) return true;
            else return false;
        }

        public int CompareTo(StarSystemObject? other)
        {
            if (this.Distance > (other?.Distance ?? 0)) return 1;
            else return -1;
        }
    }
}

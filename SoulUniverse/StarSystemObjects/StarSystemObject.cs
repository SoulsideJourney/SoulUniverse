using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal abstract class StarSystemObject :IComparable<StarSystemObject>
    {
        protected abstract char Symbol { get; }
        public abstract int Size { get; }
        public abstract double OrbitalSpeed { get; }
        public bool isNeedToRedraw = false;
        public int Distance { get; protected set;  }
        protected double Phi = 0;
        public Coordinates DrawedCoordinates = new();
        public Coordinates Coordinates = new();
        protected List<KeyValuePair<Recource, int>> recources { get; } = new();

        //Присутствующие фракции
        public List<Fraction> Fractions { get; } = new();
        public abstract void Draw();

        public void WriteInfo()
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
                Console.ForegroundColor = planet.GetColor();
                Console.Write(planet.PlanetClass);
                //Console.Write(string.Format("Планета класса {0}", planetClass));
                Console.ResetColor();
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write(string.Format("Размер: {0} км", planet.Size));
                Console.SetCursorPosition(universe_x + 2, ++row);
                Console.Write(string.Format("Расстояние до родительского тела: {0} а.е. ", planet.Distance));
            }
            else Console.Write("Неизвестный объект");

            //Информация о ресурсах
            Console.SetCursorPosition(universe_x + 2, ++row);
            Console.Write("Имеющиеся ресурсы:");
            foreach (var resource in this.recources)
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
                    Console.Write(fraction.Name);
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

        public int CompareTo(StarSystemObject? other)
        {
            if (this.Distance > (other?.Distance ?? 0)) return 1;
            else return -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulUniverse
{
    public class Enums
    {
        public enum FractionColor
        {
            //Black = ConsoleColor.Black,
            DarkBlue = ConsoleColor.DarkBlue,
            DarkGreen = ConsoleColor.DarkGreen,
            DarkCyan = ConsoleColor.DarkCyan,
            DarkRed = ConsoleColor.DarkRed,
            DarkMagenta = ConsoleColor.DarkMagenta,
            DarkYellow = ConsoleColor.DarkYellow,
            Gray = ConsoleColor.Gray,
            //DarkGray = ConsoleColor.DarkGray,
            Blue = ConsoleColor.Blue,
            //Green = ConsoleColor.Green,
            Cyan = ConsoleColor.Cyan,
            Red = ConsoleColor.Red,
            Magenta = ConsoleColor.Magenta,
            Yellow = ConsoleColor.Yellow,
            White = ConsoleColor.White
        }
        public enum DisplayMode
        {
            Universe, StarSystem, Planet, Types, Fractions
        }
        public enum FractionName
        {
            Джедаи, КосмическиеБомжи, Пираты, Рептилоиды, Ситхи, 
            Бабки_с_РПГ, РогатыеТрупоеды, КакаяТоЖуткаяХуйня, ЧетвертыйРейх, РоботыЭкстерминаторы,
            Салат, ФеечкиВинкс, СейлорМун, ОжившаяПротухшаяЕдаИзХолодильника
        }

        public enum PlanetClass
        {
            Barren, Continental, GasGiant, Ocean
        }

        public enum ResourceName
        {
            Iron, Oil, Uranium, 
        }
    }
}

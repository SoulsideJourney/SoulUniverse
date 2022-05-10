using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;

namespace SoulUniverse
{
    internal class Fraction
    {
        public string Name { get; protected set; }

        public ConsoleColor Color { get; protected set; }
        //public FractionColor Color { get; protected set; }
        public List<StarSystemObject> Colonies { get; protected set; } = new();

        //Фракция игрока
        public Fraction()
        {
            Name = "Игрок";
            Color = ConsoleColor.Green;
        }

        public Fraction(Enums.FractionName Fraction)
        {
            Name = Fraction.ToString();
            Random rnd = new Random();
            //Рандомный цвет фракции
            //Color = (ConsoleColor)Enum.GetValues(typeof(ConsoleColor)).GetValue(rnd.Next(Enum.GetValues(typeof(ConsoleColor)).Length));
            Color = (ConsoleColor)Enum.GetValues(typeof(FractionColor)).GetValue(rnd.Next(Enum.GetValues(typeof(FractionColor)).Length));
        }

        public void DoSomething()
        {

        }
    }
}

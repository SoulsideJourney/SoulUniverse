﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Program;
using static SoulUniverse.Enums;

namespace SoulUniverse.PlanetObjects
{
    public abstract class GroundObject : UniverseObject
    {
        protected abstract char Symbol { get; }
        protected abstract string Name { get; }
        public Coordinates Coordinates { get; } = new();
        public Fraction Owner { get; }
        public StarSystemObject Location { get; }
        public bool IsNeedToDraw { get; set; } = true;
        //static public abstract List<KeyValuePair<ResourceName, int>> Cost { get; }
        //public abstract void Draw();


        public virtual void Lol()
        {

        }
        public GroundObject(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
        {
            Coordinates.x = x;
            Coordinates.y = y;
            Owner = fraction;
            Location = starSystemObject;
        }

        public void WriteInfo()
        {
            lock(locker)
            {
                int row = 2;
                ResetConsoleColor();

                //Общая информация
                Console.SetCursorPosition(Universe.UniverseX + 2, row);
                Console.Write("Информация об объекте: ");
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                if (Name != null)
                {
                    Console.Write(Name + " фракции ");
                    Console.ForegroundColor = Owner.Color;
                    Console.Write(Owner.Name);
                    ResetConsoleColor();
                }
                else Console.Write("Неизвестный объект");
                InfoIsClear = false;
                ResetCursor();
            }
        }

        public override void Draw()
        {
            lock(locker)
            {
                Console.SetCursorPosition(Coordinates.x, Coordinates.y);
                if (FractionDisplayMode == Enums.DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
                else ResetConsoleColor();
                Console.Write(Symbol);
                ResetConsoleColor();
                Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
                IsNeedToDraw = false;
            }  
        }
    }
}

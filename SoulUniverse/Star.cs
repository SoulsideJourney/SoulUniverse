﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Program;

namespace SoulUniverse
{
    internal class Star : VoidObject
    {
        public StarClass starClass;
        public enum StarClass
        {
            W, O, B, A, F, G, K, M, L
        }

        public Star()
        {
            Random rnd = new Random();
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
            starClass = (StarClass)Enum.GetValues(typeof(StarClass)).GetValue(rnd.Next(Enum.GetValues(typeof(StarClass)).Length-1));
            Draw(Coordinates.x, Coordinates.y, starClass);

        }

        public Star(int x, int y)
        {
            Random rnd = new Random();
            Coordinates.x = x;
            Coordinates.y = y;
            starClass = (StarClass)Enum.GetValues(typeof(StarClass)).GetValue(rnd.Next(Enum.GetValues(typeof(StarClass)).Length-1));
            Draw(Coordinates.x, Coordinates.y, starClass);
        }

        public Star(StarClass starClass)
        {
            Random rnd = new Random();
            Coordinates.x = rnd.Next(universe_x);
            Coordinates.y = rnd.Next(universe_y);
            this.starClass = starClass;
            Draw(Coordinates.x, Coordinates.y, starClass);
            
        }

        public Star(int x, int y, StarClass starClass)
        {
            this.starClass = starClass;
            Draw(x, y, starClass);
        }
        
        public void Draw()
        {
            Draw(Coordinates.x, Coordinates.y, starClass);
        }

        protected void Draw(int x, int y, StarClass starClass)
        {
            switch (starClass)
            {
                case StarClass.W:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case StarClass.O:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case StarClass.B:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case StarClass.A:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case StarClass.F:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case StarClass.G:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case StarClass.K:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case StarClass.M:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case StarClass.L:
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

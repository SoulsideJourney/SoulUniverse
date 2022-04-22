// See https://aka.ms/new-console-template for more information
using SoulUniverse;
using System;

namespace SoulUniverse // Note: actual namespace depends on the project name.
{    
    static class Program
    {
        public const int console_x = 100;
        public const int console_y = 25;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            DrawSomeStars();
            DrawSomeWormholes();
            DrawSomeBlackHoles();

            Console.ResetColor();
            Console.SetCursorPosition(0, 30);

            //Star star1 = new(0, 0, Star.starClass.O);
            //Star star2 = new(1, 0, Star.starClass.B);
            //Star star3 = new(2, 0, Star.starClass.A);
            //Star star4 = new(3, 0, Star.starClass.F);
            //Console.ForegroundColor = ConsoleColor.DarkMagenta;
        }
        static void DrawSomeStars()
        {
            for (int i = 0; i < 100; i++)
            {
                Star star = new();
            }
        }
        static void DrawSomeWormholes()
        {
            for (int i = 0; i < 10; i++)
            {
                Wormhole wormhole = new();
            }
        }

        static void DrawSomeBlackHoles()
        {
            for (int i = 0; i < 10; i++)
            {
                BlackHole blackHole = new();
            }
        }
    }
}
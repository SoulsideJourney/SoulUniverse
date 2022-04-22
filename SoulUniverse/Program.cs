// See https://aka.ms/new-console-template for more information
using SoulUniverse;
using System;

namespace SoulUniverse // Note: actual namespace depends on the project name.
{    
    static class Program
    {
        //Границы генерации мира
        public const int universe_x = 150;
        public const int universe_y = 45;

        //Размер консоли
        public const int console_x = 200;
        public const int console_y = 50;

        //List<VoidObject> voidObjects1 = new List<VoidObject>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.SetWindowSize(console_x, console_y);
            DrawFrames();

            List<VoidObject> voidObjects = new();

            DrawSomeStars(voidObjects);
            DrawSomeWormholes(voidObjects);
            DrawSomeBlackHoles(voidObjects);

            Console.SetCursorPosition(0, 0);
            Console.Beep();

            Console.ReadKey();

            Console.ResetColor();
            Console.SetCursorPosition(0, universe_y + 1);
            (int w, int z) = Console.GetCursorPosition();

            //Star star1 = new(0, 0, Star.starClass.O);
            //Star star2 = new(1, 0, Star.starClass.B);
            //Star star3 = new(2, 0, Star.starClass.A);
            //Star star4 = new(3, 0, Star.starClass.F);
            //Console.ForegroundColor = ConsoleColor.DarkMagenta;
        }

        private static void DrawFrames()
        {
            //Горизонтальные рамки
            Console.SetCursorPosition(universe_x + 1, universe_y);
            for (int i = universe_x + 1; i <= console_x; i++)
            {
                Console.Write("-");
            }
            Console.SetCursorPosition(universe_x + 1, 0);
            for (int i = universe_x + 1; i <= console_x; i++)
            {
                Console.Write("-");
            }

            //Вертикальные рамки
            for (int i = 1; i <= universe_y - 1; i++)
            {
                Console.SetCursorPosition(universe_x + 1, i);
                Console.Write("|");
            }

            for (int i = 1; i <= universe_y - 1; i++)
            {
                Console.SetCursorPosition(console_x - 1, i);
                Console.Write("|");
            }
        }

        static void DrawSomeStars(List<VoidObject> voidObjects)
        {
            for (int i = 0; i < 100; i++)
            {
                int x;
                int y;
                bool isPositionOccupied = false;
                Random rnd = new Random();

                while (!isPositionOccupied)
                {
                    x = rnd.Next(universe_x);
                    y = rnd.Next(universe_y);
                    foreach (VoidObject obj in voidObjects)
                    {
                        if (obj.Coordinates.x == x && obj.Coordinates.y == y)
                        {
                            isPositionOccupied = true;
                            break;
                        }                        
                    }
                    if (isPositionOccupied) continue;
                    else
                    {
                        voidObjects.Add(new Star(x, y));
                        break;
                    }                         
                }              
            }
        }
        static void DrawSomeWormholes(List<VoidObject> voidObjects)
        {
            for (int i = 0; i < 10; i++)
            {
                int x;
                int y;
                bool isPositionOccupied = false;
                Random rnd = new Random();

                while (!isPositionOccupied)
                {
                    x = rnd.Next(universe_x);
                    y = rnd.Next(universe_y);
                    foreach (VoidObject obj in voidObjects)
                    {
                        if (obj.Coordinates.x == x && obj.Coordinates.y == y)
                        {
                            isPositionOccupied = true;
                            break;
                        }
                    }
                    if (isPositionOccupied) continue;
                    else
                    {
                        voidObjects.Add(new Wormhole(x, y));
                        break;
                    }
                }

            }
        }

        static void DrawSomeBlackHoles(List<VoidObject> voidObjects)
        {
            for (int i = 0; i < 10; i++)
            {
                    int x;
                    int y;
                    bool isPositionOccupied = false;
                    Random rnd = new Random();

                    while (!isPositionOccupied)
                    {
                        x = rnd.Next(universe_x);
                        y = rnd.Next(universe_y);
                        foreach (VoidObject obj in voidObjects)
                        {
                            if (obj.Coordinates.x == x && obj.Coordinates.y == y)
                            {
                                isPositionOccupied = true;
                                break;
                            }
                        }
                        if (isPositionOccupied) continue;
                        else
                        {
                            voidObjects.Add(new BlackHole(x, y));
                            break;
                        }
                    }
            }
        }




        //static void CheckAndDraw<T>(List<VoidObject> voidObjects)
        //{
        //    int x;
        //    int y;
        //    bool isPositionOccupied = false;
        //    Random rnd = new Random();

        //    while (!isPositionOccupied)
        //    {
        //        x = rnd.Next(console_x);
        //        y = rnd.Next(console_y);
        //        foreach (VoidObject obj in voidObjects)
        //        {
        //            if (obj.Coordinates.x == x && obj.Coordinates.y == y)
        //            {
        //                isPositionOccupied = true;
        //                break;
        //            }
        //        }
        //        if (isPositionOccupied) continue;
        //        else
        //        {
        //            voidObjects.Add(new T(x, y));
        //            break;
        //        }
        //    }
        //}
    }
}
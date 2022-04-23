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


            //Создание объектов
            GenerateSomeStars(voidObjects);
            GenerateSomeWormholes(voidObjects);
            GenerateSomeBlackHoles(voidObjects);



            //Считывание кнопок
            int cursor_x = 0;
            int cursor_y = 0;
            Console.SetCursorPosition(cursor_x, cursor_y);
            while (true)
            {
                //Информация об объекте

                foreach (VoidObject obj in voidObjects)
                {
                    if (obj.Coordinates.x == cursor_x && obj.Coordinates.y == cursor_y)
                    {
                        Console.ResetColor();
                        Console.SetCursorPosition(universe_x + 2, 2);
                        Console.Write("Информация об объекте: ");
                        if (obj is Star)
                        {
                            string starClass = (obj as Star).starClass.ToString();
                            Console.Write(string.Format("Звезда класса {0}", starClass));
                            Console.SetCursorPosition(universe_x + 2, 3);
                            Console.Write("Количество планет: неизвестно");
                        }
                        else if (obj is BlackHole)
                        {
                            Console.Write("Черная дыра");
                        }
                        else if (obj is Wormhole)
                        {
                            Console.Write("Червоточина");
                        }
                        else Console.Write("Неизвестный объект");

                        //Возвращение курсора
                        Console.SetCursorPosition(cursor_x, cursor_y);
                        break;
                    }
                    else
                    {
                        //Очистка инфо, если ничего не найдено
                        Console.SetCursorPosition(universe_x + 2, 2);
                        Console.Write("                                        ");
                        Console.SetCursorPosition(universe_x + 2, 3);
                        Console.Write("                                        ");
                        //Возвращение курсора
                        Console.SetCursorPosition(cursor_x, cursor_y);
                    }
                }
                

                //Считывание нажатий
                ConsoleKeyInfo consoleKeyInfo =  Console.ReadKey();

                //Стрелки
                if (consoleKeyInfo.Key == ConsoleKey.LeftArrow && cursor_x > 0)
                {
                    Console.SetCursorPosition(--cursor_x, cursor_y);
                }
                if (consoleKeyInfo.Key == ConsoleKey.RightArrow && cursor_x < universe_x)
                {
                    Console.SetCursorPosition(++cursor_x, cursor_y);
                }
                if (consoleKeyInfo.Key == ConsoleKey.UpArrow && cursor_y > 0)
                {
                    Console.SetCursorPosition(cursor_x, --cursor_y);
                }
                if (consoleKeyInfo.Key == ConsoleKey.DownArrow && cursor_y < universe_y)
                {
                    Console.SetCursorPosition(cursor_x, ++cursor_y);
                }

                //Выход
                if (consoleKeyInfo.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }

                //ConsoleCancelEventHandler? cancelKeyPress = Console.CancelKeyPress;


            }


            //Завершение
            Console.ReadKey();
            Console.ResetColor();
            Console.SetCursorPosition(0, universe_y + 1);
            (int w, int z) = Console.GetCursorPosition();

        }

        private static void DrawFrames()
        {
            //Горизонтальные рамки
            Console.SetCursorPosition(universe_x + 1, universe_y);
            for (int i = universe_x + 1; i < console_x; i++)
            {
                Console.Write("-");
            }
            Console.SetCursorPosition(universe_x + 1, 0);
            for (int i = universe_x + 1; i < console_x; i++)
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

        static void GenerateSomeStars(List<VoidObject> voidObjects)
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
        static void GenerateSomeWormholes(List<VoidObject> voidObjects)
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

        static void GenerateSomeBlackHoles(List<VoidObject> voidObjects)
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
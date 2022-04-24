// See https://aka.ms/new-console-template for more information
using SoulUniverse;
using System;

namespace SoulUniverse // Note: actual namespace depends on the project name.
{    
    static class Program
    {
        //Границы генерации мира
        public const int universe_x = 100;
        public const int universe_y = 40;

        //Размер консоли
        public const int console_x = 150;
        public const int console_y = 41;

        //List<VoidObject> voidObjects1 = new List<VoidObject>();

        static void Main(string[] args)
        {
            //Настройка консоли
            Console.Title = "Консольная Вселенная";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            #pragma warning disable CA1416 // Проверка совместимости платформы
            Console.SetWindowSize(console_x, console_y);
            Console.SetBufferSize(console_x, console_y);
            //Console.SetWindowPosition(1, 1);
            
            DrawFrames();

            List<VoidObject> voidObjects = new();


            //Создание объектов
            //GenerateSomeStars(voidObjects);
            CheckAndDraw<Star>(voidObjects, 100);
            GenerateSomeWormholes(voidObjects);
            GenerateSomeBlackHoles(voidObjects);



            //Считывание кнопок
            int cursor_x = 0;
            int cursor_y = 0;
            bool infoIsClear = true;
            VoidObject? checkedVoidObject = null;
            Console.SetCursorPosition(cursor_x, cursor_y);
            while (true)
            {
                //Информация об объекте

                foreach (VoidObject obj in voidObjects)
                {
                    if (obj.Coordinates.x == cursor_x && obj.Coordinates.y == cursor_y)
                    {
                        int row = 2;
                        Console.ResetColor();
                        Console.SetCursorPosition(universe_x + 2, row);
                        Console.Write("Информация об объекте: ");
                        if (obj is Star)
                        {
                            Star star = (Star)obj;
                            string starClass = star.starClass.ToString();
                            Console.Write(string.Format("Звезда класса {0}", starClass));
                            Console.SetCursorPosition(universe_x + 2, ++row);
                            int planets = star.starSystemObjects.Count;
                            Console.Write(string.Format("Количество планетарных тел: {0}", planets));

                            if (planets > 0)
                            {
                                WriteSystemInfo(star, row);
                            }
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
                        infoIsClear = false;
                        checkedVoidObject = obj;

                        //Возвращение курсора
                        Console.SetCursorPosition(cursor_x, cursor_y);
                        break;
                    }
                    else
                    {
                        //Очистка инфо, если ничего не найдено
                        if (!infoIsClear)
                        {
                            for (int i = 2; i < universe_y; i++)
                            {
                                Console.SetCursorPosition(universe_x + 2, i);
                                Console.Write("                                          ");
                            }
                        }
                        
                        infoIsClear = true;
                        checkedVoidObject = null;
                        //Возвращение курсора
                        Console.SetCursorPosition(cursor_x, cursor_y);
                    }
                }
                

                //Считывание нажатий
                ConsoleKeyInfo consoleKeyInfo =  Console.ReadKey(true);

                //Стрелки
                if (consoleKeyInfo.Key == ConsoleKey.LeftArrow && cursor_x > 0)
                {
                    Console.SetCursorPosition(--cursor_x, cursor_y);
                }
                else if (consoleKeyInfo.Key == ConsoleKey.RightArrow && cursor_x < universe_x)
                {
                    Console.SetCursorPosition(++cursor_x, cursor_y);
                }
                else if (consoleKeyInfo.Key == ConsoleKey.UpArrow && cursor_y > 0)
                {
                    Console.SetCursorPosition(cursor_x, --cursor_y);
                }
                else if (consoleKeyInfo.Key == ConsoleKey.DownArrow && cursor_y < universe_y)
                {
                    Console.SetCursorPosition(cursor_x, ++cursor_y);
                }

                //Enter -- отрисовка системы
                else if (consoleKeyInfo.Key == ConsoleKey.Enter && checkedVoidObject != null && checkedVoidObject is Star)
                {
                    Console.Write("OLOLO");
                    Console.Clear();
                    DrawFrames();
                    Star star = (Star)checkedVoidObject;
                    star.Draw(20, 20);
                    foreach(StarSystemObject starSystemObject in star.starSystemObjects)
                    {
                        starSystemObject.Draw();
                    }
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

        private static void WriteSystemInfo(Star star, int row)
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
            foreach (StarSystemObject starSystemObject in star.starSystemObjects)
            {
                Console.SetCursorPosition(universe_x + 2, ++row);
                string planetClass;
                if (starSystemObject is Planet)
                {
                    Planet planet = (Planet)starSystemObject;
                    planetClass = planet.planetClass.ToString();

                }
                else planetClass = "";
                Console.Write(string.Format("|{0, 2} | {1, 16} а.е | {2, 11} |", ++planetNumber, starSystemObject.Distance, planetClass));
            }
            Console.SetCursorPosition(universe_x + 2, ++row);
            Console.Write("------------------------------------------");
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




        static void CheckAndDraw<T>(List<VoidObject> voidObjects, int count) where T : VoidObject, new()
        {
            for (int i = 0; i < count; i++)
            {
                int x;
                int y;
                bool isPositionOccupied = false;
                Random rnd = new Random();

                while (!isPositionOccupied)
                {
                    x = rnd.Next(console_x);
                    y = rnd.Next(console_y);
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
                        T voidObject = new T() { };
                        voidObject.Coordinates.x = x;
                        voidObject.Coordinates.y = y;
                        voidObjects.Add(voidObject);
                        break;
                    }
                }
            }            
        }
    }
}
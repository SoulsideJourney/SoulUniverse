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

        static void Main()
        {
            //Настройка консоли
            Console.Title = "Консольная Вселенная";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //int test = Console.CursorSize;
            Console.CursorSize = 1;
            Console.SetWindowSize(console_x, console_y);
            Console.SetBufferSize(console_x, console_y);

            DrawFrames();

            List<VoidObject> voidObjects = new();

            //Создание объектов
            GenerateObjects<Star>(voidObjects, 100);
            GenerateObjects<Wormhole>(voidObjects, 10);
            GenerateObjects<BlackHole>(voidObjects, 10);
            int cursor_x = 0;
            int cursor_y = 0;
            while (true)
            {
                OpenUniverse(voidObjects, ref cursor_x, ref cursor_y);
            }            
        }

        private static void OpenUniverse(List<VoidObject> voidObjects, ref int cursor_x, ref int cursor_y)
        {
            Console.Title = "Консольная Вселенная: звездная карта";
            Console.Clear();
            DrawFrames();
            //Отрисовка объектов
            foreach (VoidObject voidObject in voidObjects)
            {
                voidObject.Draw();
            }

            //Считывание кнопок
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
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

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
                    OpenStarSystem(checkedVoidObject);
                    return;
                }

                //Выход
                if (consoleKeyInfo.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static void OpenStarSystem(VoidObject checkedVoidObject)
        {
            int starPoint = 20;
            Console.Title = "Консольная Вселенная: карта завездной системы";
            Console.Clear();
            DrawFrames();
            Star star = (Star)checkedVoidObject;
            star.Draw(starPoint, starPoint);
            foreach (StarSystemObject starSystemObject in star.starSystemObjects)
            {
                starSystemObject.Draw();
            }

            int cursor_x = starPoint;
            int cursor_y = starPoint;
            bool infoIsClear = true;
            //StarSystemObject? checkedStarSystemObject = null;
            Console.SetCursorPosition(cursor_x, cursor_y);
            while (true)
            {                
                //Информация об объекте

                foreach (StarSystemObject obj in star.starSystemObjects)
                {
                    if (cursor_x == starPoint && cursor_y == starPoint)
                    {
                        int row = 2;
                        Console.ResetColor();
                        Console.SetCursorPosition(universe_x + 2, row);
                        Console.Write("Информация об объекте: ");
                        string starClass = star.starClass.ToString();
                        Console.Write(string.Format("Звезда класса {0}", starClass));
                        Console.SetCursorPosition(universe_x + 2, ++row);
                        int planets = star.starSystemObjects.Count;
                        Console.Write(string.Format("Количество планетарных тел: {0}", planets));

                        if (planets > 0)
                        {
                            WriteSystemInfo(star, row);
                        }
                        infoIsClear = false;
                        //checkedVoidObject = obj;

                        //Возвращение курсора
                        Console.SetCursorPosition(cursor_x, cursor_y);
                        break;
                    }
                    if (obj.Coordinates.x == cursor_x - starPoint && obj.Coordinates.y == cursor_y - starPoint)
                    {
                        int row = 2;
                        Console.ResetColor();
                        Console.SetCursorPosition(universe_x + 2, row);
                        Console.Write("Информация об объекте: ");
                        if (obj is Planet)
                        {
                            Planet planet = (Planet)obj;
                            string planetClass = planet.planetClass.ToString();
                            Console.Write(string.Format("Планета класса {0}", planetClass));
                            Console.SetCursorPosition(universe_x + 2, ++row);
                            Console.Write(string.Format("Расстояние до родительского тела: {0} а.е. ", planet.Distance));
                        }
                        else Console.Write("Неизвестный объект");
                        infoIsClear = false;
                        //checkedVoidObject = obj;

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
                                Console.Write("                                              ");
                            }
                        }

                        infoIsClear = true;
                        //checkedVoidObject = null;
                        //Возвращение курсора
                        Console.SetCursorPosition(cursor_x, cursor_y);
                    }
                }

                //Считывание нажатий
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

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
                else if (consoleKeyInfo.Key == ConsoleKey.Escape)
                {
                    return;
                }
            }
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
            Console.ResetColor();
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

        static void GenerateObjects<T>(List<VoidObject> voidObjects, int number) where T : VoidObject, new()
        {
            for (int i = 0; i < number; i++)
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
                        //voidObjects.Add(new T(x, y));
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
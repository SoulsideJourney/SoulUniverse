using SoulUniverse.PlanetObjects;
using System.Diagnostics;
using static SoulUniverse.Enums;

namespace SoulUniverse
{
    class Program
    {
        public static object locker = new();
        public static Mutex mutex = new();

        //Границы генерации мира
        public const int universe_x = 100;
        public const int universe_y = 40;

        //Размер консоли
        public const int console_x = 160;
        public const int console_y = 41;

        //
        internal static bool infoIsClear = false;
        internal static bool isPaused = false;
        internal static int current_cursor_x = 0;
        internal static int current_cursor_y = 0;
        internal static DisplayMode UniverseDisplayMode = DisplayMode.Universe;
        internal static DisplayMode FractionDisplayMode = DisplayMode.Types;

        internal static int timeSelector = 4;
        internal static int[] time = { 1, 10, 100, 500, 1000, 2000 };

        //Список объектов
        static List<VoidObject> voidObjects = new();

        //Шахты
        internal static List<Mine> mines = new();

        //Список фракций
        internal static List<Fraction> NPCFractions = new();

        //Ссылки на родной мир
        static Star HomeStar;
        static Planet HomePlanet;

        //Выбранные объекты
        internal static VoidObject checkedVoidObject;
        internal static StarSystemObject checkedStarSystemObject;
        internal static GroundObject checkedGroundObject;

        static DateTime date = DateTime.Today.Date;
        //Великий рандом
        internal static Random rnd = new();

        static void Main()
        {
            //Настройка консоли
            Console.Title = "Консольная Вселенная";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
#pragma warning disable CA1416 // Проверка совместимости платформы
            Console.SetWindowSize(console_x, console_y);
            Console.SetBufferSize(console_x, console_y);
#pragma warning restore CA1416 // Проверка совместимости платформы

            //Environment.

            //Создание фракций
            foreach (FractionName fraction in Enum.GetValues(typeof(FractionName)))
            {
                NPCFractions.Add(new Fraction(fraction));
            }

            //Фракция игрока
            Fraction playerFraction = new();

            //Создание объектов
            GenerateObjects<Star>(voidObjects, 100);
            GenerateObjects<Wormhole>(voidObjects, 10);
            GenerateObjects<BlackHole>(voidObjects, 10);

            //Нахождение рандомной планеты континентального типа и выбор её в качестве родной
            Random rnd = new();
            while (true)
            {
                if (voidObjects.ElementAt(rnd.Next(voidObjects.Count)) is Star star && star.starSystemObjects.Any(obj => obj is Planet && (obj as Planet)?.PlanetClass == PlanetClass.Continental))
                {
                    HomeStar = star;
                    HomePlanet = (Planet)star.starSystemObjects.Find(obj => obj is Planet && (obj as Planet)?.PlanetClass == PlanetClass.Continental);
                    HomePlanet.Fractions.Add(playerFraction);

                    mutex.WaitOne();
                    HomePlanet.GroundObjects.Add(new MilitaryBase(rnd.Next(HomePlanet.Size), rnd.Next(HomePlanet.Size), playerFraction));
                    mutex.ReleaseMutex();
                    break;
                }
            }

            //Добавляем фракции на планеты с небольшой долей вероятности
            foreach (VoidObject voidObject in voidObjects)
            {
                if (voidObject is Star star)
                {
                    foreach (StarSystemObject starSystemObject in star.starSystemObjects)
                    {
                        if (starSystemObject is Planet planet)
                        {
                            while (true)
                            {
                                if (rnd.Next(100) < 10)
                                {
                                    Fraction fraction = NPCFractions.ElementAt(rnd.Next(NPCFractions.Count));
                                    planet.Fractions.Add(fraction);
                                    fraction.Colonies.Add(planet);
                                }
                                else break;
                            }
                        }
                    }
                }
            }

            //Поток времени
            Thread timeThread = new(SimulateTime)
            {
                Name = "Time Thread"
            };

            //Поток управления
            Thread navigateThread = new(Navigate)
            {
                Name = "Navigate Thread"
            };

            //ВКЛЮЧАЕМ ВСЁ
            timeThread.Start();
            OpenUniverse();
            navigateThread.Start();
        }

        static void Navigate()
        {
            while (true)
            {
                ReadButtons();

                //Информация об объекте
                if (UniverseDisplayMode == DisplayMode.Universe)
                {
                    checkedVoidObject = voidObjects.Find(o => (o.Coordinates.x == current_cursor_x && o.Coordinates.y == current_cursor_y));
                    lock (locker)
                    {
                        ClearInfo();
                        checkedVoidObject?.WriteInfo();
                    }
                }
                else if (UniverseDisplayMode == DisplayMode.StarSystem)
                {
                    //Информация об объекте
                    Star star = (Star)checkedVoidObject;
                    if (current_cursor_x == 20 && current_cursor_y == 20)
                    {
                        lock (locker)
                        {
                            int row = 2;
                            Console.ResetColor();
                            Console.SetCursorPosition(universe_x + 2, row);
                            Console.Write("Информация об объекте: ");
                            star?.WriteStarInfo();
                        }
                    }
                    else
                    {
                        checkedStarSystemObject = star?.starSystemObjects.Find(o => (o.Coordinates.x == current_cursor_x - 20 && o.Coordinates.y == current_cursor_y - 20));
                        lock (locker)
                        {
                            ClearInfo();
                            checkedStarSystemObject?.WriteInfo();
                        }
                    }
                }
                else if (UniverseDisplayMode == DisplayMode.Planet)
                {
                    checkedGroundObject = checkedStarSystemObject?.GroundObjects.Find(o => (o.Coordinates.x == current_cursor_x && o.Coordinates.y == current_cursor_y));
                    lock (locker)
                    {
                        ClearInfo();
                        checkedGroundObject?.WriteInfo();
                    }
                }
            }
        }

        static void SimulateTime()
        {
            Thread.Sleep(time[timeSelector]);
            while (true)
            {
                string title;
                if (UniverseDisplayMode == DisplayMode.Universe)
                {
                    title = "Консольная Вселенная: звездная карта";
                }
                else if (UniverseDisplayMode == DisplayMode.StarSystem)
                {
                    title = "Консольная Вселенная: карта звездной системы";
                }
                else //if (UniverseDisplayMode == DisplayMode.Planet)
                {
                    title = "Консольная Вселенная: карта планеты";
                }
                title += string.Format(" {0}", date.ToString());
                //Пауза отключена -- симуляция времени и действий
                if (!isPaused)
                {
                    date = date.AddDays(1);
#if DEBUG
                    Debug.Write("---Новый день в галактике...---");
#endif

                    //Расчет точек орбит планет
                    foreach (VoidObject? voidObject in voidObjects)
                    {
                        if (voidObject is Star star)
                        {
                            foreach (StarSystemObject? starSystemObject in star.starSystemObjects)
                            {
                                if (starSystemObject is Planet planet)
                                {
                                    planet.Move();
                                }
                            }
                        }
                    }
                    //Перерисовка планет
                    if (UniverseDisplayMode == DisplayMode.StarSystem)
                    {
                        if (checkedVoidObject is Star star)
                        {
                            foreach (StarSystemObject? starSystemObject in star.starSystemObjects)
                            {
                                if (starSystemObject is Planet planet && planet.isNeedToRedraw)
                                {
                                    planet.Erase();
                                    planet.Draw();
                                }
                            }
                        }
                        //OpenSystem(checkedVoidObject);
                    }

                    //Действия фракций
                    foreach (var fraction in NPCFractions)
                    {
                        fraction.DoSomething();
                    }

                    //Работа шахт
                    foreach (var mine in mines)
                    {
                        mine.Excavate();
                    }
#if DEBUG
                    Debug.Write("-------------------------------");
#endif
                }
                else title += " пауза";
                Console.Title = title;
                Thread.Sleep(time[timeSelector]);
            }
        }

        private static void OpenUniverse()
        {
            UniverseDisplayMode = DisplayMode.Universe;
            checkedStarSystemObject = null;
            Console.Title = string.Format("Консольная Вселенная: звездная карта {0}", date.ToString());
            lock (locker)
            {
                Console.Clear();
                DrawFrames();
                WriteLegend();
                //Отрисовка объектов
                DrawVoidObjects();
                current_cursor_x = checkedVoidObject?.Coordinates.x ?? 0;
                current_cursor_y = checkedVoidObject?.Coordinates.y ?? 0;
                //Первоначальная установка соответствия курсора и переменных
                ResetCursor();
            }
        }

        public static void SetCursor(int x, int y)
        {
            current_cursor_x = x;
            current_cursor_y = y;
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
        }
        public static void ResetCursor()
        {
            Console.ResetColor();
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
        }

        private static void DrawVoidObjects()
        {
            lock (locker)
            {
                foreach (VoidObject voidObject in voidObjects)
                {
                    voidObject.Draw();
                }
                Console.ResetColor();
            }
        }

        private static void OpenSystem(VoidObject checkedVoidObject)
        {
            UniverseDisplayMode = DisplayMode.StarSystem;
            Console.Title = string.Format("Консольная Вселенная: карта звездной системы {0}", date.ToString());
            lock (locker)
            {
                Console.Clear();

                WriteLegend();
                infoIsClear = true;
                DrawFrames();
                Star star = (Star)checkedVoidObject;
                DrawStarSystemObjects(star);
                current_cursor_x = checkedStarSystemObject?.Coordinates.x + 20 ?? 20;
                current_cursor_y = checkedStarSystemObject?.Coordinates.y + 20 ?? 20;
                Console.SetCursorPosition(current_cursor_x, current_cursor_y);
            }
        }

        private static void OpenPlanet(StarSystemObject starSystemObject)
        {
            UniverseDisplayMode = DisplayMode.Planet;
            Console.Title = $"Консольная Вселенная: карта планеты {date}";
            lock (locker)
            {
                Console.Clear();
                WriteLegend();
                infoIsClear = true;
                DrawFrames();
                for (int i = 0; i < starSystemObject.Size; i++)
                {
                    Console.SetCursorPosition(starSystemObject.Size, i);
                    Console.Write('|');
                }
                for (int i = 0; i < starSystemObject.Size; i++)
                {
                    Console.SetCursorPosition(i, starSystemObject.Size);
                    Console.Write('-');
                }
                starSystemObject.DrawObjects();
                SetCursor(0, 0);
            }

        }

        private static void GoHome()
        {
            lock (locker)
            {
                OpenSystem(HomeStar);
                current_cursor_x = HomePlanet.Coordinates.x + 20;
                current_cursor_y = HomePlanet.Coordinates.y + 20;
                Console.SetCursorPosition(current_cursor_x, current_cursor_y);
            }
        }

        private static void DrawStarSystemObjects(Star star)
        {
            lock (locker)
            {
                star.Draw(20, 20);
                foreach (StarSystemObject starSystemObject in star.starSystemObjects)
                {
                    if (UniverseDisplayMode != DisplayMode.StarSystem) return;
                    starSystemObject.Draw();
                }
            }
        }

        private static void DrawFrames()
        {
            lock (locker)
            {
                Console.ResetColor();
                //Вертикальные рамки
                for (int i = 0; i <= universe_y - 1; i++)
                {
                    Console.SetCursorPosition(universe_x + 1, i);
                    Console.Write("|");
                }

                for (int i = 0; i <= universe_y - 1; i++)
                {
                    Console.SetCursorPosition(console_x - 1, i);
                    Console.Write("|");
                }
                //Горизонтальные рамки
                Console.SetCursorPosition(universe_x + 2, 0);
                for (int i = universe_x + 2; i < console_x - 1; i++)
                {
                    Console.Write("-");
                }
                Console.SetCursorPosition(universe_x + 2, universe_y - 1);
                for (int i = universe_x + 2; i < console_x - 1; i++)
                {
                    Console.Write("-");
                }
            }
        }

        static void GenerateObjects<T>(List<VoidObject> voidObjects, int number) where T : VoidObject, new()
        {
            for (int i = 0; i < number; i++)
            {
                int x;
                int y;
                bool isPositionOccupied = false;
                Random rnd = new();

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
                        T voidObject = new() { };
                        voidObject.Coordinates.x = x;
                        voidObject.Coordinates.y = y;
                        voidObjects.Add(voidObject);
                        break;
                    }
                }
            }
        }

        static void ClearInfo()
        {
            lock (locker)
            {
                int offset = 9;
                //Очистка инфо, если ничего не найдено
                if (!infoIsClear)
                {
                    for (int i = 2; i < universe_y - offset; i++)
                    {
                        Console.SetCursorPosition(universe_x + 2, i);
                        Console.Write("                                               ");
                    }
                }
                infoIsClear = true;
                //Возвращение курсора
                Console.SetCursorPosition(current_cursor_x, current_cursor_y);
            }
        }

        private static void WriteLegend()
        {
            lock (locker)
            {
                int offset = 9;
                Console.SetCursorPosition(universe_x + 2, universe_y - offset);
                for (int i = universe_x + 2; i < console_x - 1; i++)
                {
                    Console.Write("-");
                }
                Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
                Console.Write("Управление:");
                Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
                Console.Write("\u2190\u2191\u2192\u2193 -- навигация");
                if (UniverseDisplayMode == DisplayMode.Universe) Console.Write(", Enter -- войти в систему");
                if (UniverseDisplayMode == DisplayMode.StarSystem) Console.Write(", Enter -- открыть карту объекта");
                Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
                Console.Write("Режимы отображения: T -- классы объектов, F -- фракции");
                Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
                Console.Write("B -- строить");
                Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
                Console.Write("D -- дипломатический статус");
                Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
                Console.Write("H -- к родной планете, +- -- скорость симуляции");
                Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
                Console.Write("P -- пауза");
                if (UniverseDisplayMode == DisplayMode.Universe) Console.Write(", Esc -- выход");
                else if (UniverseDisplayMode == DisplayMode.StarSystem) Console.Write(", Esc -- к звездной карте");
                else if (UniverseDisplayMode == DisplayMode.Planet) Console.Write(", Esc -- к карте системы");
            }
        }

        static void ReadButtons()
        {
            //Считывание нажатий
            ConsoleKey consoleKey = Console.ReadKey(true).Key;

            //Стрелки
            if (consoleKey == ConsoleKey.LeftArrow && current_cursor_x > 0)
            {
                Console.SetCursorPosition(--current_cursor_x, current_cursor_y);
            }
            else if (consoleKey == ConsoleKey.RightArrow && current_cursor_x < universe_x)
            {
                Console.SetCursorPosition(++current_cursor_x, current_cursor_y);
            }
            else if (consoleKey == ConsoleKey.UpArrow && current_cursor_y > 0)
            {
                Console.SetCursorPosition(current_cursor_x, --current_cursor_y);
            }
            else if (consoleKey == ConsoleKey.DownArrow && current_cursor_y < universe_y)
            {
                Console.SetCursorPosition(current_cursor_x, ++current_cursor_y);
            }

            //Режимы отображения
            else if (consoleKey == ConsoleKey.T)
            {
                //Режим отображения классов объектов
                if (FractionDisplayMode != DisplayMode.Types)
                {
                    FractionDisplayMode = DisplayMode.Types;
                    if (UniverseDisplayMode == DisplayMode.Universe)
                    {
                        DrawVoidObjects();
                    }
                    else if (UniverseDisplayMode == DisplayMode.StarSystem)
                    {
                        //DrawStarSystemObjects((Star)checkedVoidObject);
                        (checkedVoidObject as Star)!.DrawStarSystemObjects();
                    }
                    else if (UniverseDisplayMode == DisplayMode.Planet)
                    {
                        //DrawStarSystemObjects((Star)checkedVoidObject);
                        checkedStarSystemObject!.DrawObjects();
                    }
                }
            }
            else if (consoleKey == ConsoleKey.F)
            {
                //Режим отображения принадлежности к фракциям
                if (FractionDisplayMode != DisplayMode.Fractions)
                {
                    FractionDisplayMode = DisplayMode.Fractions;
                    if (UniverseDisplayMode == DisplayMode.Universe)
                    {
                        DrawVoidObjects();
                    }
                    else if (UniverseDisplayMode == DisplayMode.StarSystem)
                    {
                        //DrawStarSystemObjects((Star)checkedVoidObject);
                        (checkedVoidObject as Star)!.DrawStarSystemObjects();
                    }
                    else if (UniverseDisplayMode == DisplayMode.Planet)
                    {
                        //DrawStarSystemObjects((Star)checkedVoidObject);
                        checkedStarSystemObject!.DrawObjects();
                    }
                }
            }

            //Переход к родной системе
            else if (consoleKey == ConsoleKey.H)
            {
                checkedVoidObject = HomeStar;
                //OpenSystem(checkedVoidObject);
                GoHome();
            }

            //Enter -- вход в систему или планету
            else if (consoleKey == ConsoleKey.Enter)
            {
                if (UniverseDisplayMode == DisplayMode.Universe)
                {
                    if (checkedVoidObject != null && checkedVoidObject is Star)
                    {
                        OpenSystem(checkedVoidObject);
                    }
                }
                else if (UniverseDisplayMode == DisplayMode.StarSystem)
                {
                    if (checkedVoidObject != null && checkedStarSystemObject is Planet)
                    {
                        OpenPlanet(checkedStarSystemObject);
                    }
                }
            }

            //+- -- регулирование скорости течения времени
            else if (consoleKey == ConsoleKey.Subtract)
            {
                if (timeSelector < time.Length - 1) timeSelector++;
            }
            else if (consoleKey == ConsoleKey.Add)
            {
                if (timeSelector > 0) timeSelector--;
            }

            //P -- пауза
            else if (consoleKey == ConsoleKey.P || consoleKey == ConsoleKey.Pause)
            {
                isPaused = !isPaused;
            }

            //Выход
            else if (consoleKey == ConsoleKey.Escape)
            {
                if (UniverseDisplayMode == DisplayMode.StarSystem)
                {
                    OpenUniverse();
                }
                else if (UniverseDisplayMode == DisplayMode.Planet)
                {
                    OpenSystem(checkedVoidObject);
                }
                else Environment.Exit(0);
            }
        }
    }
}
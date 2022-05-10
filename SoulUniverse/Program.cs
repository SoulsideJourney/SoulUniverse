using static SoulUniverse.Enums;

namespace SoulUniverse
{
    class Program
    {
        //Границы генерации мира
        public const int universe_x = 100;
        public const int universe_y = 40;

        //Размер консоли
        public const int console_x = 150;
        public const int console_y = 41;

        //
        internal static bool infoIsClear = false;
        internal static bool isPaused = false;
        internal static int current_cursor_x = 0;
        internal static int current_cursor_y = 0;
        internal static DisplayMode UniverseDisplayMode = DisplayMode.Universe;
        internal static DisplayMode FractionDisplayMode = DisplayMode.Types;

        //Список объектов
        static List<VoidObject> voidObjects = new();

        //Список фракций
        internal static List<Fraction> NPCFractions = new();

        //Ссылки на родной мир
        static Star HomeStar;
        static Planet HomePlanet;

        //Выбранные объекты
        internal static VoidObject checkedVoidObject;
        internal static StarSystemObject checkedStarSystemObject;

        static DateTime date = DateTime.Today.Date;

        static void Main()
        {
            //Настройка консоли
            Console.Title = "Консольная Вселенная";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
#pragma warning disable CA1416 // Проверка совместимости платформы
            Console.SetWindowSize(console_x, console_y);
            Console.SetBufferSize(console_x, console_y);
#pragma warning restore CA1416 // Проверка совместимости платформы

            //Создание фракций
            foreach (FractionName fraction in Enum.GetValues(typeof(FractionName)))
            {
                NPCFractions.Add(new Fraction(fraction));
            }

            //Создание объектов
            GenerateObjects<Star>(voidObjects, 100);
            GenerateObjects<Wormhole>(voidObjects, 10);
            GenerateObjects<BlackHole>(voidObjects, 10);

            //Нахождение рандомной планеты континентального типа и выбор её в качестве родной
            Random random = new Random();
            while (true)
            {
                if (voidObjects.ElementAt(random.Next(voidObjects.Count)) is Star star && star.starSystemObjects.Any(obj => obj is Planet && (obj as Planet)?.PlanetClass == PlanetClass.Continental))
                {
                    HomeStar = star;
                    HomePlanet = (Planet)star.starSystemObjects.Find(obj => obj is Planet && (obj as Planet)?.PlanetClass == PlanetClass.Continental);
                    HomePlanet.Fractions.Add(new Fraction());
                    break;
                }
            }

            //Поток времени
            Thread timeThread = new(SimulateTime);

            //Поток управления
            Thread navigateThread = new(Navigate);

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
                    ClearInfo();
                    checkedVoidObject?.WriteInfo();
                }
                else if (UniverseDisplayMode == DisplayMode.StarSystem)
                {
                    //Информация об объекте
                    Star star = (Star)checkedVoidObject;
                    if (current_cursor_x == 20 && current_cursor_y == 20)
                    {
                        int row = 2;
                        Console.ResetColor();
                        Console.SetCursorPosition(universe_x + 2, row);
                        Console.Write("Информация об объекте: ");
                        star?.WriteStarInfo();

                    }
                    else
                    {
                        checkedStarSystemObject = star.starSystemObjects.Find(o => (o.Coordinates.x == current_cursor_x - 20 && o.Coordinates.y == current_cursor_y - 20));
                        ClearInfo();
                        checkedStarSystemObject?.WriteInfo();
                    }
                }
            }
        }

        static void SimulateTime()
        {
            Thread.Sleep(1000);
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

                    //Расчет точек отбит планет
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
                }
                else title += " пауза";
                Console.Title = title;
                Thread.Sleep(100);
            }
        }

        private static void OpenUniverse()
        {
            UniverseDisplayMode = DisplayMode.Universe;
            checkedStarSystemObject = null;
            Console.Title = string.Format("Консольная Вселенная: звездная карта {0}", date.ToString());
            Console.Clear();
            DrawFrames();
            WriteLegend();
            //Отрисовка объектов
            DrawVoidObjects();
            current_cursor_x = checkedVoidObject?.Coordinates.x ?? 0;
            current_cursor_y = checkedVoidObject?.Coordinates.y ?? 0;
            //Первоначальная установка соответствия курсора и переменных
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
        }

        private static void DrawVoidObjects()
        {
            foreach (VoidObject voidObject in voidObjects)
            {
                voidObject.Draw();
            }
            Console.ResetColor();
        }

        private static void OpenSystem(VoidObject checkedVoidObject)
        {
            UniverseDisplayMode = DisplayMode.StarSystem;
            Console.Title = string.Format("Консольная Вселенная: карта звездной системы {0}", date.ToString());
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

        private static void OpenPlanet(StarSystemObject starSystemObject)
        {
            UniverseDisplayMode = DisplayMode.Planet;
            Console.Title = string.Format("Консольная Вселенная: карта планеты {0}", date.ToString());
            Console.Clear();
            WriteLegend();
            infoIsClear = true;
            DrawFrames();
            Planet planet = (Planet)starSystemObject;
            planet.DrawMacro();
        }

        private static void GoHome()
        {
            OpenSystem(HomeStar);
            current_cursor_x = HomePlanet.Coordinates.x + 20;
            current_cursor_y = HomePlanet.Coordinates.y + 20;
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
        }

        private static void DrawStarSystemObjects(Star star)
        {
            star.Draw(20, 20);
            foreach (StarSystemObject starSystemObject in star.starSystemObjects)
            {
                starSystemObject.Draw();
            }
        }

        //private static void OpenSystem(StarSystemObject starSystemObject)
        //{
        //    var parent = starSystemObject.
        //}

        private static void DrawFrames()
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

            //Легенда
            //WriteLegend();
            infoIsClear = true;
            //Возвращение курсора
            Console.SetCursorPosition(current_cursor_x, current_cursor_y);
        }

        private static void WriteLegend()
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
            Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
            Console.Write("T -- режим отображения классов объектов");
            Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
            Console.Write("F -- режим отображения фракций");
            Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
            Console.Write("D -- дипломатический статус");
            Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
            Console.Write("H -- к родной планете");
            if (UniverseDisplayMode == DisplayMode.Universe) Console.Write(", Esc -- выход");
            else if (UniverseDisplayMode == DisplayMode.StarSystem) Console.Write(", Esc -- к звездной карте");
            Console.SetCursorPosition(universe_x + 2, universe_y - --offset);
            Console.Write("P -- пауза");
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
                    if (UniverseDisplayMode == DisplayMode.StarSystem)
                    {
                        DrawStarSystemObjects((Star)checkedVoidObject);
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
                    if (UniverseDisplayMode == DisplayMode.StarSystem)
                    {
                        DrawStarSystemObjects((Star)checkedVoidObject);
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
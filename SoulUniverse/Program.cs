using SoulUniverse.PlanetObjects;
using System.Diagnostics;
using System.Xml.Linq;
using static SoulUniverse.Enums;

namespace SoulUniverse
{
    static class Program
    {
        public static object locker = new();
        public static Mutex mutex = new();

        /// <summary> Размер консоли по оси X </summary>
        private const int ConsoleX = 200;

        /// <summary> Размер консоли по оси Y </summary>
        private const int ConsoleY = 50;

        //
        internal static bool infoIsClear = false;
        internal static bool isPaused = false;
        internal static int current_cursor_x = 0;
        internal static int current_cursor_y = 0;
        internal static DisplayMode UniverseDisplayMode = DisplayMode.Universe;
        internal static DisplayMode FractionDisplayMode = DisplayMode.Types;

        internal static int timeSelector = 4;
        internal static int[] time = { 1, 10, 100, 500, 1000, 2000 };

        //Выбранные объекты
        internal static VoidObject checkedVoidObject;
        internal static StarSystemObject checkedStarSystemObject;
        internal static GroundObject checkedGroundObject;

        /// <summary> Великий рандом </summary>
        internal static readonly Random Rnd = new();

        static void Main()
        {
            //Настройка консоли
            Console.Title = "Консольная Вселенная";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.SetWindowSize(ConsoleX, ConsoleY);
            Console.SetBufferSize(ConsoleX, ConsoleY);

            Universe.Initialize();

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
                    checkedVoidObject = Universe.VoidObjects.Find(o => (o.Coordinates.x == current_cursor_x && o.Coordinates.y == current_cursor_y));
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
                            Console.SetCursorPosition(Universe.UniverseX + 2, row);
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

        /// <summary>Симуляция времени</summary>
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
                title += $" {Universe.CurrentDate} танков: {Universe.Tanks.Count}, шахт: {Universe.Mines.Count}, заводов: {Universe.Factories.Count}";
                //Пауза отключена -- симуляция времени и действий
                if (!isPaused)
                {
                    Universe.CurrentDate = Universe.CurrentDate.AddDays(1);
#if DEBUG
                    Debug.Write("---Новый день в галактике...---");
#endif

                    //Расчет точек орбит планет
                    foreach (VoidObject? voidObject in Universe.VoidObjects)
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
                            foreach (StarSystemObject starSystemObject in star.starSystemObjects)
                            {
                                if (starSystemObject is Planet planet && planet.IsNeedToRedraw)
                                {
                                    planet.Erase();
                                    planet.Draw();
                                }
                            }
                        }
                        //OpenSystem(checkedVoidObject);
                    }

                    if (UniverseDisplayMode == DisplayMode.Planet)
                    {
                        //if (checkedStarSystemObject is Planet planet)
                        //{
                        foreach (GroundObject groundObject in checkedStarSystemObject.GroundObjects)
                        {
                            //Отрисовка новых объектов
                            if (groundObject.IsNeedToDraw)
                            {
                                groundObject.Draw();
                            }
                            //Перерисовка движущихся наземных объектов
                            if (groundObject is Tank tank && tank.IsNeedToRedraw)
                            {
                                tank.Erase();
                                tank.Draw();
                            }
                        }
                        //}
                    }

                    //Действия фракций
                    foreach (var fraction in Universe.NPCFractions)
                    {
                        fraction.DoSomething();
                    }

                    //Работа шахт
                    foreach (var mine in Universe.Mines)
                    {
                        mine.Excavate();
                    }

                    //Работа заводов
                    foreach (var factory in Universe.Factories)
                    {
                        if (factory.Owner.IsEnoughToBuildTank()
                            && factory.Location.GroundObjects.Where(_ => _ is Tank && _.Owner == factory.Owner).Count() < 10
                            && !factory.Location.IsPlaceOccupied(factory.Coordinates.x + 1, factory.Coordinates.y))
                        {
                            factory.BuildTank();
                            Debug.WriteLine(string.Format($"Насекомые из {factory.Owner.Name} построили ТАНК! Будет война"));
                        }
                    }

                    //Движение танков
                    foreach (var tank in Universe.Tanks)
                    {
                        tank.Move();
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

        /// <summary>Просмотр Вселенной</summary>
        private static void OpenUniverse()
        {
            UniverseDisplayMode = DisplayMode.Universe;
            checkedStarSystemObject = null;
            Console.Title = string.Format("Консольная Вселенная: звездная карта {0}", Universe.CurrentDate.ToString());
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
                foreach (VoidObject voidObject in Universe.VoidObjects)
                {
                    voidObject.Draw();
                }
                Console.ResetColor();
            }
        }

        private static void OpenSystem(VoidObject checkedVoidObject)
        {
            UniverseDisplayMode = DisplayMode.StarSystem;
            Console.Title = $"Консольная Вселенная: карта звездной системы {Universe.CurrentDate}";
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
            Console.Title = $"Консольная Вселенная: карта планеты {Universe.CurrentDate}";
            lock (locker)
            {
                Console.Clear();
                WriteLegend();
                infoIsClear = true;
                DrawFrames();
                for (int i = 0; i <= starSystemObject.Size; i++)
                {
                    Console.SetCursorPosition(starSystemObject.Size + 1, i);
                    Console.Write('|');
                }
                for (int i = 0; i <= starSystemObject.Size; i++)
                {
                    Console.SetCursorPosition(i, starSystemObject.Size + 1);
                    Console.Write('-');
                }
                starSystemObject.DrawObjects();
                SetCursor(0, 0);
            }

        }

        /// <summary> Переход к родной системе </summary>
        private static void GoHome()
        {
            lock (locker)
            {
                OpenSystem(Universe.HomeStar);
                current_cursor_x = Universe.HomePlanet.Coordinates.x + 20;
                current_cursor_y = Universe.HomePlanet.Coordinates.y + 20;
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
                for (int i = 0; i < Universe.UniverseY; i++)
                {
                    Console.SetCursorPosition(Universe.UniverseX + 1, i);
                    Console.Write("|");
                }

                for (int i = 0; i < Universe.UniverseY; i++)
                {
                    Console.SetCursorPosition(ConsoleX - 1, i);
                    Console.Write("|");
                }

                //Горизонтальные рамки
                Console.SetCursorPosition(Universe.UniverseX + 2, 0);
                for (int i = Universe.UniverseX + 2; i < ConsoleX - 1; i++)
                {
                    Console.Write("-");
                }

                //Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - 1);
                //for (int i = Universe.UniverseX + 2; i < ConsoleX - 1; i++)
                //{
                //    Console.Write("-");
                //}

                Console.SetCursorPosition(0, Universe.UniverseY);
                for (int i = 0; i < ConsoleX; i++)
                {
                    Console.Write("-");
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
                    for (int i = 2; i < Universe.UniverseY - offset; i++)
                    {
                        Console.SetCursorPosition(Universe.UniverseX + 2, i);
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
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - offset);
                for (int i = Universe.UniverseX + 2; i < ConsoleX - 1; i++)
                {
                    Console.Write("-");
                }
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - --offset);
                Console.Write("Управление:");
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - --offset);
                Console.Write("\u2190\u2191\u2192\u2193 -- навигация");
                if (UniverseDisplayMode == DisplayMode.Universe) Console.Write(", Enter -- войти в систему");
                if (UniverseDisplayMode == DisplayMode.StarSystem) Console.Write(", Enter -- открыть карту объекта");
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - --offset);
                Console.Write("Режимы отображения: T -- классы объектов, F -- фракции");
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - --offset);
                Console.Write("B -- строить");
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - --offset);
                Console.Write("D -- дипломатический статус");
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - --offset);
                Console.Write("H -- к родной планете, +- -- скорость симуляции");
                Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - --offset);
                Console.Write("P -- пауза");
                if (UniverseDisplayMode == DisplayMode.Universe) Console.Write(", Esc -- выход");
                else if (UniverseDisplayMode == DisplayMode.StarSystem) Console.Write(", Esc -- к звездной карте");
                else if (UniverseDisplayMode == DisplayMode.Planet) Console.Write(", Esc -- к карте системы");
            }
        }

        /// <summary> Считывание нажатий клавиш </summary>
        private static void ReadButtons()
        {
            //Считывание нажатий
            ConsoleKey consoleKey = Console.ReadKey(true).Key;

            //Стрелки
            if (consoleKey == ConsoleKey.LeftArrow && current_cursor_x > 0)
            {
                Console.SetCursorPosition(--current_cursor_x, current_cursor_y);
            }
            else if (consoleKey == ConsoleKey.RightArrow && current_cursor_x < Universe.UniverseX)
            {
                Console.SetCursorPosition(++current_cursor_x, current_cursor_y);
            }
            else if (consoleKey == ConsoleKey.UpArrow && current_cursor_y > 0)
            {
                Console.SetCursorPosition(current_cursor_x, --current_cursor_y);
            }
            else if (consoleKey == ConsoleKey.DownArrow && current_cursor_y < Universe.UniverseY)
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
                        checkedStarSystemObject.DrawObjects();
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
                checkedVoidObject = Universe.HomeStar;
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

            //B -- Режим строительства
            else if (consoleKey == ConsoleKey.B)
            {
                
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
            else if (consoleKey is ConsoleKey.P or ConsoleKey.Pause)
            {
                isPaused = !isPaused;
            }

            //Тест
            else if (consoleKey is ConsoleKey.Z)
            {
                
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
                //else Environment.Exit(0);
            }
        }
    }
}
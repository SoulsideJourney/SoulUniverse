using SoulUniverse.PlanetObjects;
using SoulUniverse.StarSystemObjects;
using SoulUniverse.VoidObjects;
using System.Diagnostics;
using static SoulUniverse.Enums;

namespace SoulUniverse;

// Для нормальной работы в Windows 11 нужно сделать:
// Настройки -> Система -> Для разработчиков -> Узел консоли Windows
internal static class Program
{
    public static readonly object Locker = new();
    public static readonly Mutex Mutex = new();

    /// <summary> Размер консоли по оси X </summary>
    private const int ConsoleX = 200;

    /// <summary> Размер консоли по оси Y </summary>
    private const int ConsoleY = 50;

    private const int LegendOffsetFromBottom = 8;

    public static bool InfoIsClear;

    private static bool _isPaused;

    private static bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            SetConsoleTitle();
        }
    }

    private static int _currentCursorX;
    private static int _currentCursorY;

    public static int CurrentCursorX
    {
        get => _currentCursorX;
        private set
        {
            _currentCursorX = value;
            SetConsoleTitle();
        }
    }

    public static int CurrentCursorY
    {
        get => _currentCursorY;
        private set
        {
            _currentCursorY = value;
            SetConsoleTitle();
        }
    }

    private static DisplayMode _universeDisplayMode = DisplayMode.Universe;

    private static DisplayMode UniverseDisplayMode
    {
        get => _universeDisplayMode;
        set
        {
            _universeDisplayMode = value;
            SetConsoleTitle();
        }
    }

    internal static DisplayMode FractionDisplayMode = DisplayMode.Types;

    private static int _timeSelector = 4;
    private static readonly int[] TimeSpans = [1, 10, 100, 500, 1000, 2000];

    //Выбранные объекты
    internal static VoidObject? CheckedVoidObject;
    internal static StarSystemObject? CheckedStarSystemObject;
    internal static GroundObject? CheckedGroundObject;

    /// <summary> Великий рандом </summary>
    internal static readonly Random Rnd = new();

    public static void ResetConsoleColor()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void SetConsoleTitle()
    {
        string title = "Консольная Вселенная";

        if (UniverseDisplayMode == DisplayMode.Universe)
        {
            title += ": звездная карта";
        }
        else if (UniverseDisplayMode == DisplayMode.StarSystem)
        {
            title += ": карта звездной системы";
        }
        else
        {
            title += ": карта планеты";
        }
        title += $" {Universe.CurrentDate} танков: {Universe.Tanks.Count}, шахт: {Universe.Mines.Count}, заводов: {Universe.Factories.Count}";
        title += $" X:{CurrentCursorX} Y:{CurrentCursorY}";
        if (IsPaused) title += " пауза";
        Console.Title = title;
    }

    private static void Main()
    {
        //Настройка консоли
        SetConsoleTitle();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.SetWindowSize(ConsoleX, ConsoleY);
        Console.SetBufferSize(ConsoleX, ConsoleY);

        ResetConsoleColor();

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

    private static void Navigate()
    {
        while (true)
        {
            ReadButtons();

            //Информация об объекте
            if (UniverseDisplayMode == DisplayMode.Universe)
            {
                CheckedVoidObject = Universe.VoidObjects.Find(o => o.Coordinates.x == CurrentCursorX && o.Coordinates.y == CurrentCursorY);
                lock (Locker)
                {
                    ClearInfo();
                    CheckedVoidObject?.WriteInfo();
                }
            }
            else if (UniverseDisplayMode == DisplayMode.StarSystem)
            {
                //Информация об объекте
                Star star = (Star)CheckedVoidObject!;
                if (CurrentCursorX == 20 && CurrentCursorY == 20)
                {
                    lock (Locker)
                    {
                        ResetConsoleColor();
                        Console.SetCursorPosition(Universe.UniverseX + 2, 2);
                        Console.Write("Информация об объекте: ");
                        star.WriteStarInfo();
                    }
                }
                else
                {
                    CheckedStarSystemObject = star.StarSystemObjects.Find(o => o.Coordinates.x == CurrentCursorX - 20 && o.Coordinates.y == CurrentCursorY - 20);
                    lock (Locker)
                    {
                        ClearInfo();
                        CheckedStarSystemObject?.WriteInfo();
                    }
                }
            }
            else if (UniverseDisplayMode == DisplayMode.Planet)
            {
                CheckedGroundObject = CheckedStarSystemObject?.GroundObjects.Find(o => o.Coordinates.x == CurrentCursorX && o.Coordinates.y == CurrentCursorY);
                lock (Locker)
                {
                    ClearInfo();
                    CheckedGroundObject?.WriteInfo();
                }
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>Симуляция времени</summary>
    static void SimulateTime()
    {
        Thread.Sleep(TimeSpans[_timeSelector]);
        while (true)
        {

            //Пауза отключена -- симуляция времени и действий
            if (!IsPaused)
            {
                Universe.CurrentDate = Universe.CurrentDate.AddDays(1);
#if DEBUG
                Debug.WriteLine("------Новый день в галактике, как быстро летит время...------");
#endif

                //Расчет точек орбит планет
                foreach (VoidObject voidObject in Universe.VoidObjects)
                {
                    if (voidObject is Star star)
                    {
                        foreach (StarSystemObject starSystemObject in star.StarSystemObjects)
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
                    if (CheckedVoidObject is Star star)
                    {
                        foreach (StarSystemObject starSystemObject in star.StarSystemObjects)
                        {
                            if (starSystemObject is Planet { IsNeedToRedraw: true } planet)
                            {
                                planet.Erase();
                                planet.Draw();
                            }
                        }
                    }
                }

                if (UniverseDisplayMode == DisplayMode.Planet)
                {
                    foreach (GroundObject groundObject in CheckedStarSystemObject!.GroundObjects)
                    {
                        //Отрисовка новых объектов
                        if (groundObject is GroundProperty { IsNeedToDraw: true } groundProperty)
                        {
                            groundProperty.Draw();
                        }
                        //Перерисовка движущихся наземных объектов
                        if (groundObject is Tank { IsNeedToRedraw: true } tank)
                        {
                            tank.Erase();
                            tank.Draw();
                        }
                    }
                }

                //Действия фракций
                foreach (var fraction in Universe.NpcFractions)
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
                        && factory.Location.GroundObjects.Count(o => o is Tank tank && tank.Owner == factory.Owner) < factory.Location.PlacesCount * 0.8
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
            }
            Thread.Sleep(TimeSpans[_timeSelector]);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>Просмотр Вселенной</summary>
    private static void OpenUniverse()
    {
        UniverseDisplayMode = DisplayMode.Universe;
        CheckedStarSystemObject = null;
        lock (Locker)
        {
            Console.Clear();
            DrawFrames();
            WriteLegend();
            //Отрисовка объектов
            DrawVoidObjects();
            CurrentCursorX = CheckedVoidObject?.Coordinates.x ?? 0;
            CurrentCursorY = CheckedVoidObject?.Coordinates.y ?? 0;
            //Первоначальная установка соответствия курсора и переменных
            ResetCursor();
        }
    }

    /// <summary> Установить курсор </summary>
    public static void SetCursor(int x, int y)
    {
        CurrentCursorX = x;
        CurrentCursorY = y;
        Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
    }

    public static void ResetCursor()
    {
        ResetConsoleColor();
        Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
    }

    private static void DrawVoidObjects()
    {
        lock (Locker)
        {
            foreach (VoidObject voidObject in Universe.VoidObjects)
            {
                voidObject.Draw();
            }
            ResetConsoleColor();
        }
    }

    private static void OpenSystem(VoidObject checkedVoidObject)
    {
        UniverseDisplayMode = DisplayMode.StarSystem;
        lock (Locker)
        {
            Console.Clear();

            WriteLegend();
            InfoIsClear = true;
            DrawFrames();
            Star star = (Star)checkedVoidObject;
            DrawStarSystemObjects(star);
            CurrentCursorX = CheckedStarSystemObject?.Coordinates.x + 20 ?? 20;
            CurrentCursorY = CheckedStarSystemObject?.Coordinates.y + 20 ?? 20;
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    private static void OpenPlanet(StarSystemObject starSystemObject)
    {
        UniverseDisplayMode = DisplayMode.Planet;
        lock (Locker)
        {
            Console.Clear();
            WriteLegend();
            InfoIsClear = true;
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
        lock (Locker)
        {
            OpenSystem(Universe.HomeStar);
            CurrentCursorX = Universe.HomePlanet.Coordinates.x + 20;
            CurrentCursorY = Universe.HomePlanet.Coordinates.y + 20;
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    private static void DrawStarSystemObjects(Star star)
    {
        lock (Locker)
        {
            star.Draw(20, 20);
            foreach (StarSystemObject starSystemObject in star.StarSystemObjects)
            {
                if (UniverseDisplayMode != DisplayMode.StarSystem) return;
                starSystemObject.Draw();
            }
        }
    }

    /// <summary> Отрисовка рамок/// </summary>
    private static void DrawFrames()
    {
        lock (Locker)
        {
            ResetConsoleColor();
            //Вертикальные рамки
            for (int i = 0; i < Universe.UniverseY; i++)
            {
                Console.SetCursorPosition(Universe.UniverseX + 1, i);
                Console.Write("|");
            }

            for (int i = 0; i < Universe.UniverseY; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 1, i);
                Console.Write("|");
            }

            //Горизонтальные рамки
            Console.SetCursorPosition(Universe.UniverseX + 2, 0);
            for (int i = Universe.UniverseX + 2; i < Console.WindowWidth - 1; i++)
            {
                Console.Write("-");
            }

            //Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - 1);
            //for (int i = Universe.UniverseX + 2; i < ConsoleX - 1; i++)
            //{
            //    Console.Write("-");
            //}

            Console.SetCursorPosition(0, Universe.UniverseY);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("-");
            }
        }
    }

    private static void ClearInfo()
    {
        lock (Locker)
        {
            int offset = 9;
            //Очистка инфо, если ничего не найдено
            if (!InfoIsClear)
            {
                for (int i = 2; i < Universe.UniverseY - offset; i++)
                {
                    Console.SetCursorPosition(Universe.UniverseX + 2, i);
                    Console.Write("                                               ");
                }
            }
            InfoIsClear = true;
            //Возвращение курсора
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }
    
    /// <summary> Отрисовка легенды </summary>
    private static void WriteLegend()
    {
        lock (Locker)
        {
            int offset = 0;
            ResetConsoleColor();
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom + 1));
            for (int i = Universe.UniverseX + 2; i < Console.WindowWidth - 1; i++)
            {
                Console.Write("-");
            }
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("Управление:");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("\u2190\u2191\u2192\u2193 -- навигация");
            if (UniverseDisplayMode == DisplayMode.Universe) Console.Write(", Enter -- войти в систему");
            if (UniverseDisplayMode == DisplayMode.StarSystem) Console.Write(", Enter -- открыть карту объекта");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("Режимы отображения: T -- классы объектов, F -- фракции");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("B -- строить");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("D -- дипломатический статус");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("H -- к родной планете, +- -- скорость симуляции");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
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
        if (consoleKey == ConsoleKey.LeftArrow && CurrentCursorX > 0)
        {
            Console.SetCursorPosition(--CurrentCursorX, CurrentCursorY);
        }
        else if (consoleKey == ConsoleKey.RightArrow && CurrentCursorX < Universe.UniverseX)
        {
            Console.SetCursorPosition(++CurrentCursorX, CurrentCursorY);
        }
        else if (consoleKey == ConsoleKey.UpArrow && CurrentCursorY > 0)
        {
            Console.SetCursorPosition(CurrentCursorX, --CurrentCursorY);
        }
        else if (consoleKey == ConsoleKey.DownArrow && CurrentCursorY < Universe.UniverseY)
        {
            Console.SetCursorPosition(CurrentCursorX, ++CurrentCursorY);
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
                    (CheckedVoidObject as Star)!.DrawStarSystemObjects();
                }
                else if (UniverseDisplayMode == DisplayMode.Planet)
                {
                    CheckedStarSystemObject!.DrawObjects();
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
                    (CheckedVoidObject as Star)!.DrawStarSystemObjects();
                }
                else if (UniverseDisplayMode == DisplayMode.Planet)
                {
                    //DrawStarSystemObjects((Star)checkedVoidObject);
                    CheckedStarSystemObject!.DrawObjects();
                }
            }
        }

        //Переход к родной системе
        else if (consoleKey == ConsoleKey.H)
        {
            CheckedVoidObject = Universe.HomeStar;
            GoHome();
        }

        //Enter -- вход в систему или планету
        else if (consoleKey == ConsoleKey.Enter)
        {
            if (UniverseDisplayMode == DisplayMode.Universe)
            {
                if (CheckedVoidObject is Star)
                {
                    OpenSystem(CheckedVoidObject);
                }
            }
            else if (UniverseDisplayMode == DisplayMode.StarSystem)
            {
                if (CheckedVoidObject != null && CheckedStarSystemObject is Planet)
                {
                    OpenPlanet(CheckedStarSystemObject);
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
            if (_timeSelector < TimeSpans.Length - 1) _timeSelector++;
        }
        else if (consoleKey == ConsoleKey.Add)
        {
            if (_timeSelector > 0) _timeSelector--;
        }

        //P -- пауза
        else if (consoleKey is ConsoleKey.P or ConsoleKey.Pause)
        {
            IsPaused = !IsPaused;
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
                OpenSystem(CheckedVoidObject!);
            }
            //else Environment.Exit(0);
        }
    }
}
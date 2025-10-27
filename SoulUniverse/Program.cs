using SoulUniverse.Objects.PlanetObjects;
using SoulUniverse.Objects.StarSystemObjects;
using SoulUniverse.Objects.VoidObjects;
using System.Diagnostics;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;

namespace SoulUniverse;

//TODO попробовать ловить клики мыши
//TODO сделать сохранение мира, например, сериализацию в JSON

// Для нормальной работы в Windows 11 нужно сделать:
// Настройки -> Система -> Для разработчиков -> Узел консоли Windows
internal static class Program
{
    public static readonly object Locker = new();
    public static readonly Mutex Mutex = new();

    private const int LegendOffsetFromBottom = 8;

    /// <summary> Отступ при отрисовке звезды в карте системы </summary>
    public const int StarOffset = 20;

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

    private static Scale _selectedScale = Scale.Universe;

    private static Scale SelectedScale
    {
        get => _selectedScale;
        set
        {
            _selectedScale = value;
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

        if (SelectedScale == Scale.Universe)
        {
            title += ": звездная карта";
        }
        else if (SelectedScale == Scale.StarSystem)
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
            if (SelectedScale == Scale.Universe)
            {
                CheckedVoidObject = Universe.VoidObjects.Find(o => o.Coordinates.X == CurrentCursorX && o.Coordinates.Y == CurrentCursorY);
                lock (Locker)
                {
                    ClearInfo();
                    CheckedVoidObject?.WriteInfo();
                }
            }
            else if (SelectedScale == Scale.StarSystem)
            {
                //Информация об объекте
                Star star = (Star)CheckedVoidObject!;
                
                //TODO ну и шляпа тут написана
                if (CurrentCursorX == StarOffset && CurrentCursorY == StarOffset)
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
                    CheckedStarSystemObject = star.StarSystemObjects.Find(o => o.Coordinates.X == CurrentCursorX - StarOffset && o.Coordinates.Y == CurrentCursorY - StarOffset);
                    lock (Locker)
                    {
                        ClearInfo();
                        CheckedStarSystemObject?.WriteInfo();
                    }
                }
            }
            else if (SelectedScale == Scale.Planet)
            {
                //TODO инфо о шахтах на местах месторождений
                CheckedGroundObject = CheckedStarSystemObject?.GroundObjects.Find(o => o.Coordinates.X == CurrentCursorX && o.Coordinates.Y == CurrentCursorY);
                lock (Locker)
                {
                    ClearInfo();
                    CheckedGroundObject?.WriteInfo();
                }
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>Симуляция времени: расчеты движения и выполнение действий объектов </summary>
    private static void SimulateTime()
    {
        Thread.Sleep(TimeSpans[_timeSelector]);
        while (true)
        {

            //Пауза отключена -- симуляция времени и действий
            if (!IsPaused)
            {
                Universe.CurrentDate = Universe.CurrentDate.AddDays(1);

                Debug.WriteLine("------Новый день в галактике, как быстро летит время...------");

                //Расчет точек орбит планет
                foreach (VoidObject voidObject in Universe.VoidObjects)
                {
                    if (voidObject is Star star)
                    {
                        foreach (StarSystemObject starSystemObject in star.StarSystemObjects)
                        {
                            starSystemObject.Move();
                        }
                    }
                }
                //Перерисовка планет
                if (SelectedScale == Scale.StarSystem)
                {
                    if (CheckedVoidObject is Star star)
                    {
                        foreach (StarSystemObject starSystemObject in star.StarSystemObjects)
                        {
                            if (starSystemObject is { IsNeedToRedraw: true })
                            {
                                starSystemObject.Erase();
                                starSystemObject.Draw();
                            }
                        }
                    }
                }

                if (SelectedScale == Scale.Planet)
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
                    factory.Work();
                }

                //Движение танков
                foreach (var tank in Universe.Tanks)
                {
                    tank.Move();
                    tank.TryFire();
                }

                Universe.Tanks.RemoveAll(t => t.Health <= 0);
            }
            Thread.Sleep(TimeSpans[_timeSelector]);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>Просмотр Вселенной</summary>
    private static void OpenUniverse()
    {
        SelectedScale = Scale.Universe;
        CheckedStarSystemObject = null;
        lock (Locker)
        {
            Console.Clear();
            DrawFrames();
            WriteLegend();
            //Отрисовка объектов
            DrawVoidObjects();
            CurrentCursorX = CheckedVoidObject?.Coordinates.X ?? 0;
            CurrentCursorY = CheckedVoidObject?.Coordinates.Y ?? 0;
            //Первоначальная установка соответствия курсора и переменных
            ResetCursor();
        }
    }

    /// <summary>
    /// Вернуть курсор в сохраненную позицию
    /// </summary>
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
        SelectedScale = Scale.StarSystem;
        lock (Locker)
        {
            Console.Clear();

            WriteLegend();
            InfoIsClear = true;
            DrawFrames();
            Star star = (Star)checkedVoidObject;
            DrawStarSystemObjects(star);
            CurrentCursorX = CheckedStarSystemObject?.Coordinates.X + StarOffset ?? StarOffset;
            CurrentCursorY = CheckedStarSystemObject?.Coordinates.Y + StarOffset ?? StarOffset;
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    private static void OpenPlanet(StarSystemObject starSystemObject)
    {
        SelectedScale = Scale.Planet;
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
            CurrentCursorX = Universe.HomePlanet.Coordinates.X + StarOffset;
            CurrentCursorY = Universe.HomePlanet.Coordinates.Y + StarOffset;
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    private static void DrawStarSystemObjects(Star star)
    {
        lock (Locker)
        {
            star.Draw(StarOffset, StarOffset);
            foreach (StarSystemObject starSystemObject in star.StarSystemObjects)
            {
                if (SelectedScale != Scale.StarSystem) return;
                starSystemObject.Draw();
            }
        }
    }

    /// <summary> Отрисовка рамок </summary>
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
                    Console.Write("                                                                 ");
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
            if (SelectedScale == Scale.Universe) Console.Write(", Enter -- войти в систему");
            if (SelectedScale == Scale.StarSystem) Console.Write(", Enter -- открыть карту объекта");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("V -- режимы отображения (классы объектов/фракции)");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("B -- строить");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("D -- дипломатический статус");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("H -- к родной планете, +- -- скорость симуляции");
            Console.SetCursorPosition(Universe.UniverseX + 2, Universe.UniverseY - (LegendOffsetFromBottom - offset++));
            Console.Write("P -- пауза");
            if (SelectedScale == Scale.Universe) Console.Write(", Esc -- выход");
            else if (SelectedScale == Scale.StarSystem) Console.Write(", Esc -- к звездной карте");
            else if (SelectedScale == Scale.Planet) Console.Write(", Esc -- к карте системы");
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
        else if (consoleKey == ConsoleKey.V)
        {
            FractionDisplayMode = FractionDisplayMode == DisplayMode.Types ? DisplayMode.Fractions : DisplayMode.Types;

            //Режим отображения классов объектов
            if (FractionDisplayMode == DisplayMode.Types)
            {
                if (SelectedScale == Scale.Universe)
                {
                    DrawVoidObjects();
                }
                else if (SelectedScale == Scale.StarSystem)
                {
                    //DrawStarSystemObjects((Star)checkedVoidObject);
                    (CheckedVoidObject as Star)!.DrawStarSystemObjects();
                }
                else if (SelectedScale == Scale.Planet)
                {
                    CheckedStarSystemObject!.DrawObjects();
                }
            }

            //Режим отображения принадлежности к фракциям
            else if (FractionDisplayMode == DisplayMode.Fractions)
            {
                if (SelectedScale == Scale.Universe)
                {
                    DrawVoidObjects();
                }
                else if (SelectedScale == Scale.StarSystem)
                {
                    //DrawStarSystemObjects((Star)checkedVoidObject);
                    (CheckedVoidObject as Star)!.DrawStarSystemObjects();
                }
                else if (SelectedScale == Scale.Planet)
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
            if (SelectedScale == Scale.Universe)
            {
                if (CheckedVoidObject is Star)
                {
                    OpenSystem(CheckedVoidObject);
                }
            }
            else if (SelectedScale == Scale.StarSystem)
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
            //TODO
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
            if (SelectedScale == Scale.StarSystem)
            {
                OpenUniverse();
            }
            else if (SelectedScale == Scale.Planet)
            {
                OpenSystem(CheckedVoidObject!);
            }
            //else Environment.Exit(0);
        }
    }
}
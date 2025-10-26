using SoulUniverse.Objects.PlanetObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.StarSystemObjects;

public sealed class Planet : StarSystemObject
{
    public const int MinSize = 10;
    public const int MaxSize = 25;

    protected override char Symbol => '\u25CF'; //●

    public override int Size { get; } = Rnd.Next(MinSize, MaxSize);

    public override double OrbitalSpeed => 107_000;

    public PlanetClass PlanetClass { get; }

    /// <summary> Цвет планеты в зависимости от её класса </summary>
    public override ConsoleColor Color =>
        PlanetClass switch
        {
            PlanetClass.Barren => ConsoleColor.DarkYellow,
            PlanetClass.Continental => ConsoleColor.Green,
            PlanetClass.GasGiant => ConsoleColor.DarkBlue,
            PlanetClass.Hycean => ConsoleColor.Cyan,
            _ => ConsoleColor.White
        };

    public Planet(int distance) : base(distance)
    {
        //Генерация ресурсов
        foreach (ResourceName res in Enum.GetValues(typeof(ResourceName)))
        {
            Resources.Add(res, Rnd.Next(100000000));
        }
        PlanetClass = (PlanetClass)Rnd.Next(Enum.GetValues(typeof(PlanetClass)).Length);

        //Генерация месторождений
        int i = 0;
        while (i < Size + Rnd.Next(-2, 2))
        {
            GenerateDeposit(ResourceName.Iron);

            i++;
        }

        i = 0;
        while (i < Size / 4 + Rnd.Next(-2, 2))
        {
            GenerateDeposit(ResourceName.Oil);

            i++;
        }

        i = 0;
        while (i < Size / 10 + Rnd.Next(-2, 2))
        {
            GenerateDeposit(ResourceName.Uranium);

            i++;
        }
    }

    private void GenerateDeposit(ResourceName resource)
    {
        int x = Rnd.Next(Size);
        int y = Rnd.Next(Size);
        if (!IsPlaceOccupied(x, y))
        {
            Deposit deposit = new Deposit(new Coordinates(x, y), this, resource);

            Program.Mutex.WaitOne();
            GroundObjects.Add(deposit);
            Deposits.Add(deposit);
            Program.Mutex.ReleaseMutex();
        }
    }

    public override void Draw()
    {
        lock (Locker)
        {
            //Отрисовка планеты
            if (FractionDisplayMode == DisplayMode.Fractions)
            {
                Console.ForegroundColor = IsColonized ? Fractions.ElementAt(0).Color : ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = Color;
            }
            Console.SetCursorPosition(20 + Coordinates.X, 20 + Coordinates.Y);
            Console.Write(Symbol);
            DrawnCoordinates = Coordinates;
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            IsNeedToRedraw = false;
        }
    }
    public void AddGroundObjects(Fraction fraction)
    {
        int x = Rnd.Next(Size);
        int y = Rnd.Next(Size);
        if (!IsPlaceOccupied(x, y)) MilitaryBase.New(x, y, fraction, this);
    }

    protected override void WriteSelfInfo(ref int row)
    {
        Console.Write("Планета класса ");
        Console.ForegroundColor = Color;
        Console.Write(PlanetClass);
        ResetConsoleColor();
        Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
        Console.Write($"Диаметр: {Size * 1000} км");
        Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
        Console.Write($"Расстояние до родительского тела: {OrbitRadius} а.е. ");
        Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
    }
}
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
    public override int Size { get; init; } = Rnd.Next(MinSize, MaxSize);
    public override double OrbitalSpeed => 107_000;
    public PlanetClass PlanetClass { get; init; }

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
            Recources.Add(res, Rnd.Next(100000000));
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
        ////Отрисовка орбиты
        //ResetConsoleColor();
        //int absoluteX;
        //int absoluteY;

        //for (int i = 0; i <= Distance; i++)
        //{
        //    //absoluteX = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - i));
        //    absoluteX = i;
        //    absoluteY = (int)Math.Round(Math.Sqrt(Math.Pow(Distance, 2) - Math.Pow(absoluteX, 2)));
        //    SetCursor(20 + absoluteX, 20 + absoluteY);
        //    Write("\u00B7");
        //    SetCursor(20 + absoluteX * -1, 20 + absoluteY);
        //    Write("\u00B7");
        //    SetCursor(20 + absoluteX * -1, 20 + absoluteY * -1);
        //    Write("\u00B7");
        //    SetCursor(20 + absoluteX, 20 + absoluteY * -1);
        //    Write("\u00B7");
        //}
        lock (Locker)
        {
            //Отрисовка планеты
            if (FractionDisplayMode == DisplayMode.Fractions)
            {
                if (IsColonized) Console.ForegroundColor = Fractions.ElementAt(0).Color;
                else Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = Color;
            }
            SetCursor(20 + Coordinates.X, 20 + Coordinates.Y);
            Write(Symbol);
            DrawnCoordinates = Coordinates;
            SetCursor(CurrentCursorX, CurrentCursorY);
            IsNeedToRedraw = false;
        }
    }

    //public void DrawMacro()
    //{
    //    int center = 20;
    //    Console.ForegroundColor = this.GetColor();
    //    for (int i = center - 12; i <= center + 12; i++)
    //    {
    //        SetCursor(i, center);
    //        Write('*');
    //    }
    //    SetCursor(center, center - 6);
    //    for (int i = center - 6; i <= center + 6; i++)
    //    {
    //        SetCursor(center, i);
    //        Write('*');
    //    }
    //    ResetConsoleColor();
    //}


    //public void Move()
    //{
    //    //Угловая скорость
    //    double w = OrbitalSpeed / ((double)Distance * 150_000_000); // рад/ч
    //    double t = 24; //часов
    //    Phi = (Phi + w * t) % (2 * Math.PI);
    //    int newX = (int)Math.Round(Distance * Math.Cos(Phi)); // а. е.
    //    int newY = (int)Math.Round(Distance * Math.Sin(Phi)); // а. е.
    //    if (Coordinates.x != newX || Coordinates.y != newY)
    //    {
    //        Coordinates.x = newX;
    //        Coordinates.y = newY;
    //        IsNeedToRedraw = true;
    //    }

    //}

    public void AddGroundObjects(Fraction fraction)
    {
        int x = Rnd.Next(Size);
        int y = Rnd.Next(Size);
        if (!IsPlaceOccupied(x, y)) MilitaryBase.New(x, y, fraction, this);
    }
}
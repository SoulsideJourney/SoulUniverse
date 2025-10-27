using SoulUniverse.Interfaces;
using SoulUniverse.Objects.PlanetObjects;
using SoulUniverse.Objects.VoidObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.StarSystemObjects;

/// <summary> Небесное тело в планетарной системе (планетного масштаба) </summary>
public abstract class StarSystemObject : UniverseObject, IComparable<StarSystemObject>, IMovable
{
    public bool IsNeedToRedraw { get; set; }

    public string Name { get; } = PlanetNames.Names[Rnd.Next(PlanetNames.Names.Length)];

    /// <summary>Орбитальная скорость в км/ч</summary>
    public abstract double OrbitalSpeed { get; }

    /// <summary> Радиус орбиты в а.е. </summary>
    public int OrbitRadius { get; }

    /// <summary> Родительский объект (звезда как правило) </summary>
    public Star ParentObject { get; }

    public int PlacesCount => Size * Size;

    /// <summary> Диаметр объекта, в тыс. км. </summary>
    public abstract int Size { get; }

    protected abstract char Symbol { get; }

    /// <summary> Текущий угол орбиты </summary>
    private double _phi;

    public Coordinates DrawnCoordinates { get; set; }
    public Coordinates Coordinates;

    public readonly List<GroundObject> GroundObjects = [];
    public List<Deposit> Deposits => GroundObjects.Where(o => o is Deposit).Cast<Deposit>().ToList();

    public bool IsColonized => Fractions.Length > 0;

    public Dictionary<ResourceName, int> Resources { get; } = new();

    /// <summary> Присутствующие фракции </summary>
    public Fraction[] Fractions => GroundObjects.Where(o => o is GroundProperty).Cast<GroundProperty>()
        .Select(p => p.Owner).Distinct().ToArray();

    public abstract ConsoleColor Color { get; }

    public StarSystemObject(Star parentObject, int distance)
    {
        OrbitRadius = distance;
        ParentObject = parentObject;
        _phi = Rnd.NextDouble() * 2 * Math.PI;
        Coordinates.X = (int)Math.Round(OrbitRadius * Math.Cos(_phi));
        Coordinates.Y = (int)Math.Round(OrbitRadius * Math.Sin(_phi));
    }

    public void DrawObjects()
    {
        Program.Mutex.WaitOne();
        foreach (var obj in GroundObjects)
        {
            obj.Draw();
        }
        Program.Mutex.ReleaseMutex();
    }

    protected virtual void WriteSelfInfo(ref int row)
    {
        Console.Write("Неизвестный объект");
    }

    public void WriteInfo()
    {
        lock (Locker)
        {
            int row = 2;
            ResetConsoleColor();

            //Общая информация
            Console.SetCursorPosition(Universe.UniverseX + 2, row);
            Console.Write("Информация об объекте: ");
            Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
            WriteSelfInfo(ref row);

            //Информация о ресурсах
            Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
            Console.Write("Имеющиеся ресурсы:");
            foreach (var resource in Resources)
            {
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write($"{resource.Key}: {resource.Value}");
            }
            InfoIsClear = false;

            //Информация о фракциях
            if (Fractions.Length > 0)
            {
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write("Фракции:");
                foreach (var fraction in Fractions)
                {
                    Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                    Console.ForegroundColor = fraction.Color;
                    Console.Write($"{fraction.Name} ({fraction.Property.Count(p => p.Location == this)} объектов)");
                }
                ResetConsoleColor();
            }
            else
            {
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write("На планете нет фракций");
            }

            //Возвращение курсора
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    public void Move()
    {
        //Угловая скорость
        double w = OrbitalSpeed / ((double)OrbitRadius * 150_000_000); // рад/ч
        _phi = (_phi + w * 24) % (2 * Math.PI); //24 -- часов
        int newX = (int)Math.Round(OrbitRadius * Math.Cos(_phi)); // а. е.
        int newY = (int)Math.Round(OrbitRadius * Math.Sin(_phi)); // а. е.
        if (Coordinates.X != newX || Coordinates.Y != newY)
        {
            Coordinates = new Coordinates(newX, newY);
            IsNeedToRedraw = true;
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
            Console.SetCursorPosition(StarOffset + Coordinates.X, StarOffset + Coordinates.Y);
            Console.Write(Symbol);
            DrawnCoordinates = Coordinates;
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            IsNeedToRedraw = false;
        }
    }

    public void Erase()
    {
        lock (Locker)
        {
            Console.SetCursorPosition(StarOffset + DrawnCoordinates.X, StarOffset + DrawnCoordinates.Y);
            Console.Write(' ');
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    /// <summary>Занято ли место на планете</summary>
    public bool IsPlaceOccupied(int x, int y)
    {
        if (GroundObjects.Any(o => o.Coordinates.X == x && o.Coordinates.Y == y)) return true;
        else return false;
    }

    public int CompareTo(StarSystemObject? other)
    {
        if (OrbitRadius > (other?.OrbitRadius ?? 0)) return 1;
        return -1;
    }

    protected void GenerateResources()
    {
        //Генерация ресурсов и месторождений
        int i = 0;
        bool generated = false;
        while (i < Size + Rnd.Next(-2, 2))
        {
            GenerateDeposit(ResourceName.Iron);
            generated = true;
            i++;
        }
        if (generated) Resources.Add(ResourceName.Iron, Rnd.Next(100000000));

        i = 0;
        generated = false;
        while (i < Size / 2 + Rnd.Next(-2, 2))
        {
            GenerateDeposit(ResourceName.Oil);
            generated = true;
            i++;
        }
        if (generated) Resources.Add(ResourceName.Oil, Rnd.Next(1000000));

        i = 0;
        generated = false;
        while (i < Size / 5 + Rnd.Next(-2, 2))
        {
            GenerateDeposit(ResourceName.Uranium);
            generated = true;

            i++;
        }
        if (generated) Resources.Add(ResourceName.Uranium, Rnd.Next(100000));
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
}
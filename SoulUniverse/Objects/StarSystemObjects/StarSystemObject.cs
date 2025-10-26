using SoulUniverse.Interfaces;
using SoulUniverse.Objects.PlanetObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.StarSystemObjects;

/// <summary> Небесное тело в планетарной системе (планетного масштаба) </summary>
public abstract class StarSystemObject : UniverseObject, IComparable<StarSystemObject>, IMovable
{
    protected abstract char Symbol { get; }

    /// <summary> Диаметр объекта, в тыс. км. </summary>
    public abstract int Size { get; }

    public int PlacesCount => Size * Size;

    /// <summary>Орбитальная скорость в км/ч</summary>
    public abstract double OrbitalSpeed { get; }

    public bool IsNeedToRedraw { get; set; }

    /// <summary> Радиус орбиты в а.е. </summary>
    public int OrbitRadius { get; }

    /// <summary> Текущий угол орбиты </summary>
    private double _phi;

    public Coordinates DrawnCoordinates { get; set; }
    public Coordinates Coordinates;
    public readonly List<GroundObject> GroundObjects = [];
    public List<Deposit> Deposits => GroundObjects.Where(o => o is Deposit).Cast<Deposit>().ToList();

    public bool IsColonized => Fractions.Count > 0;

    public Dictionary<ResourceName, int> Resources { get; } = new();

    /// <summary> Присутствующие фракции </summary>
    public List<Fraction> Fractions { get; } = [];

    public abstract ConsoleColor Color { get; }

    public StarSystemObject(int distance)
    {
        OrbitRadius = distance;
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
            if (Fractions.Count > 0)
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
}
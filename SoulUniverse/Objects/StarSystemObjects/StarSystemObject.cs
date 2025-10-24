using SoulUniverse.Interfaces;
using SoulUniverse.Objects.PlanetObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.StarSystemObjects;

public abstract class StarSystemObject : UniverseObject, IComparable<StarSystemObject>, IMovable
{
    protected abstract char Symbol { get; }
    public abstract int Size { get; init; }

    public int PlacesCount => Size * Size;

    /// <summary>Орбитальная скорость в км/ч</summary>
    public abstract double OrbitalSpeed { get; }

    public bool IsNeedToRedraw { get; set; }

    public int Distance { get; }

    /// <summary> Текущий угол орбиты </summary>
    private double _phi = 0;

    //public Coordinates DrawedCoordinates = new();
    public Coordinates DrawnCoordinates { get; set; } = new();
    public readonly Coordinates Coordinates = new();
    public readonly List<GroundObject> GroundObjects = new();
    public List<Deposit> Deposits => GroundObjects.Where(o => o is Deposit).Cast<Deposit>().ToList();

    public bool IsColonized => Fractions.Count > 0;

    public Dictionary<ResourceName, int> Recources { get; } = new();


    //Присутствующие фракции
    public List<Fraction> Fractions { get; } = new();

    public abstract ConsoleColor Color { get; }

    public StarSystemObject(int distance)
    {
        Distance = distance;
        _phi = Rnd.NextDouble() * 2 * Math.PI;
        Coordinates.x = (int)Math.Round(Distance * Math.Cos(_phi));
        Coordinates.y = (int)Math.Round(Distance * Math.Sin(_phi));
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
            if (this is Planet planet)
            {
                //string planetClass = planet.PlanetClass.ToString();
                Console.Write("Планета класса ");
                Console.ForegroundColor = planet.Color;
                Console.Write(planet.PlanetClass);
                //Console.Write(string.Format("Планета класса {0}", planetClass));
                ResetConsoleColor();
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write($"Размер: {planet.Size} км");
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write($"Расстояние до родительского тела: {planet.Distance} а.е. ");
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);              
            }
            else Console.Write("Неизвестный объект");

            //Информация о ресурсах
            Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
            Console.Write("Имеющиеся ресурсы:");
            foreach (var resource in Recources)
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
        double w = OrbitalSpeed / ((double)Distance * 150_000_000); // рад/ч
        double t = 24; //часов
        _phi = (_phi + w * t) % (2 * Math.PI);
        int newX = (int)Math.Round(Distance * Math.Cos(_phi)); // а. е.
        int newY = (int)Math.Round(Distance * Math.Sin(_phi)); // а. е.
        if (Coordinates.x != newX || Coordinates.y != newY)
        {
            Coordinates.x = newX;
            Coordinates.y = newY;
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
                if (IsColonized) Console.ForegroundColor = Fractions.ElementAt(0).Color;
                else Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = Color;
            }
            Console.SetCursorPosition(20 + Coordinates.x, 20 + Coordinates.y);
            Console.Write(Symbol);
            DrawnCoordinates.x = Coordinates.x;
            DrawnCoordinates.y = Coordinates.y;
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            IsNeedToRedraw = false;
        }
    }

    public void Erase()
    {
        lock (Locker)
        {
            Console.SetCursorPosition(20 + DrawnCoordinates.x, 20 + DrawnCoordinates.y);
            Console.Write(' ');
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    /// <summary>Занято ли место на планете</summary>
    public bool IsPlaceOccupied(int x, int y)
    {
        if (GroundObjects.Any(o => o.Coordinates.x == x && o.Coordinates.y == y)) return true;
        else return false;
    }

    public int CompareTo(StarSystemObject? other)
    {
        if (Distance > (other?.Distance ?? 0)) return 1;
        else return -1;
    }
}
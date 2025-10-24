using SoulUniverse.Interfaces;
using SoulUniverse.StarSystemObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.PlanetObjects;

internal class Tank : GroundProperty, IMovable, IBuildable
{
    //public override Fraction Owner { get; }

    //public override StarSystemObject Location { get; }

    protected override char Symbol => '♣';

    protected override string Name => "Танк мать его";

    public static List<KeyValuePair<ResourceName, int>> Cost { get; } =
    [
        new(ResourceName.Iron, 10),
        new(ResourceName.Uranium, 1),
        new(ResourceName.Oil, 5),
    ];

    public bool IsNeedToRedraw { get; set; } = true;
    public Coordinates DrawnCoordinates { get; set; } = new();

    private Tank(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject) { }
        
    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        Tank tank = new(x, y, fraction, starSystemObject);

        foreach (var res in fraction.Resources)
        {
            fraction.Resources[res.Key] = res.Value - Cost.Find(r => r.Key == res.Key).Value;
        }

        Program.Mutex.WaitOne();
        Universe.Tanks.Add(tank);
        starSystemObject.GroundObjects.Add(tank);
        Program.Mutex.ReleaseMutex();
    }

    /// <summary>Рандомное движение танка на одно поле</summary>
    public void Move()
    {
        //if (DrawnCoordinates.x != 0 && DrawnCoordinates.y != 0)
        //{

        //}
        int x = Coordinates.x;
        int y = Coordinates.y;
        _ = Rnd.Next(4) switch
        {
            0 => x < Location.Size ? x += 1 : x -= 1,
            1 => x > 0 ? x -= 1 : x += 1,
            2 => y < Location.Size ? y += 1 : y -= 1,
            _ => y > 0 ? y -= 1 : y += 1,
        };

        if (!((Planet)Location).IsPlaceOccupied(x, y))
        {
            Coordinates.x = x;
            Coordinates.y = y;
            IsNeedToRedraw = true;
        }
    }

    public override void Draw()
    {
        lock (Locker)
        {
            Console.SetCursorPosition(Coordinates.x, Coordinates.y);
            if (FractionDisplayMode == DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
            else ResetConsoleColor();
            Console.Write(Symbol);
            DrawnCoordinates.x = Coordinates.x;
            DrawnCoordinates.y = Coordinates.y;
            ResetConsoleColor();
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            IsNeedToRedraw = false;
        }
    }

    public void Erase()
    {
        lock (Locker)
        {
            Console.SetCursorPosition(DrawnCoordinates.x, DrawnCoordinates.y);
            Console.Write(' ');
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }
}
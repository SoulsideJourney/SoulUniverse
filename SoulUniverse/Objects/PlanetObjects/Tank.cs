using SoulUniverse.Interfaces;
using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.PlanetObjects;

/// <summary> Танк </summary>
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
    public Coordinates DrawnCoordinates { get; set; }

    private Tank(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, fraction, starSystemObject) { }
        
    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        Tank tank = new(new Coordinates(x, y), fraction, starSystemObject);

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
        int x = Coordinates.X;
        int y = Coordinates.Y;
        _ = Rnd.Next(4) switch
        {
            0 => x < Location.Size ? x += 1 : x -= 1,
            1 => x > 0 ? x -= 1 : x += 1,
            2 => y < Location.Size ? y += 1 : y -= 1,
            _ => y > 0 ? y -= 1 : y += 1,
        };

        if (!((Planet)Location).IsPlaceOccupied(x, y))
        {
            Coordinates = new Coordinates(x, y);
            IsNeedToRedraw = true;
        }
    }

    /// <summary> Стрелять во всё что движется и не движется </summary>
    public void TryFire()
    {
        var potentialTargets = Location.GroundObjects.Where(o => o is GroundProperty p && p.Owner != Owner).Cast<GroundProperty>().ToList();

        //Сначала танки, потом всё остальное
        var target = potentialTargets.FirstOrDefault(t => t is Tank) ?? potentialTargets.First();

        Fire(target);
    }

    private void Fire(GroundProperty target)
    {

    }

    public override void Draw()
    {
        lock (Locker)
        {
            Console.SetCursorPosition(Coordinates.X, Coordinates.Y);
            if (FractionDisplayMode == DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
            else ResetConsoleColor();
            Console.Write(Symbol);
            DrawnCoordinates = Coordinates;
            ResetConsoleColor();
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            IsNeedToRedraw = false;
        }
    }

    public void Erase()
    {
        lock (Locker)
        {
            Console.SetCursorPosition(DrawnCoordinates.X, DrawnCoordinates.Y);
            Console.Write(' ');
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }
}
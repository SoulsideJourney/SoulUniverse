using SoulUniverse.Interfaces;
using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.Enums;

namespace SoulUniverse.Objects.PlanetObjects;

/// <summary> Колониальный корабль </summary>
internal class ColonialShip : GroundProperty, IBuildable
{
    public ColonialShip(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, fraction, starSystemObject) { }

    public static List<KeyValuePair<ResourceName, int>> BuildCost { get; } =
    [
        new(ResourceName.Iron, 10000),
        new(ResourceName.Uranium, 500),
        new(ResourceName.Oil, 1000)
    ];

    public override int Health { get; set; } = 1000;

    protected override string Name => "Колониальный корабль";

    protected override char Symbol => '◊';

    /// <summary> Колониальный корабль выполнил свою миссию и превратился в стационарную базу </summary>
    private bool IsActivated { get; set; }

    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        ColonialShip ship = new(new Coordinates(x, y), fraction, starSystemObject);

        foreach (var res in fraction.Resources)
        {
            fraction.Resources[res.Key] = res.Value - BuildCost.Find(r => r.Key == res.Key).Value;
        }

        Program.Mutex.WaitOne();
        //Universe.Tanks.Add(tank); //TODO
        starSystemObject.GroundObjects.Add(ship);
        Program.Mutex.ReleaseMutex();
    }
}
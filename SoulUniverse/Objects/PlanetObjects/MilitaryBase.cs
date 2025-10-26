using SoulUniverse.Interfaces;
using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.Enums;

namespace SoulUniverse.Objects.PlanetObjects;

internal class MilitaryBase : GroundProperty, IBuildable
{
    public static List<KeyValuePair<ResourceName, int>> BuildCost { get; } =
    [
        new(ResourceName.Iron, 10000),
        new(ResourceName.Uranium, 50),
        new(ResourceName.Oil, 1000),
    ];

    public override int Health { get; set; } = 1000;

    protected override string Name => "Военная база";

    protected override char Symbol => '#';

    private MilitaryBase(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, fraction, starSystemObject) { }

    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject, bool isFree = false)
    {
        MilitaryBase @base = new(new Coordinates(x, y), fraction, starSystemObject);

        if (!isFree)
        {
            foreach (var res in fraction.Resources)
            {
                fraction.Resources[res.Key] = res.Value - BuildCost.Find(r => r.Key == res.Key).Value;
            }
        }
        Program.Mutex.WaitOne();
        starSystemObject.GroundObjects.Add(@base);
        Program.Mutex.ReleaseMutex();
    }
}
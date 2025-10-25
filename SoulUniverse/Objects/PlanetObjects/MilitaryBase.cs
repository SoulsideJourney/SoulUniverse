using SoulUniverse.Interfaces;
using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.Enums;

namespace SoulUniverse.Objects.PlanetObjects;

internal class MilitaryBase : GroundProperty, IBuildable
{
    protected override char Symbol => '#';
    protected override string Name => "Военная база";

    public static List<KeyValuePair<ResourceName, int>> BuildCost { get; } =
    [
        new(ResourceName.Iron, 10000),
        new(ResourceName.Uranium, 50),
        new(ResourceName.Oil, 1000),
    ];

    private MilitaryBase(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, fraction, starSystemObject) { }

    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        MilitaryBase @base = new(new Coordinates(x, y), fraction, starSystemObject);

        foreach (var res in fraction.Resources)
        {
            fraction.Resources[res.Key] = res.Value - BuildCost.Find(r => r.Key == res.Key).Value;
        }
        Program.Mutex.WaitOne();
        starSystemObject.GroundObjects.Add(@base);
        Program.Mutex.ReleaseMutex();
    }
}
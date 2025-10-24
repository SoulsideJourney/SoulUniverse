using SoulUniverse.Interfaces;
using SoulUniverse.StarSystemObjects;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.PlanetObjects;

internal class MilitaryBase : GroundProperty, IBuildable
{
    protected override char Symbol => '#';
    protected override string Name => "Военная база";

    public static List<KeyValuePair<ResourceName, int>> Cost { get; } =
    [
        new(ResourceName.Iron, 10000),
        new(ResourceName.Uranium, 50),
        new(ResourceName.Oil, 1000),
    ];

    private MilitaryBase(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject) { }

    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        MilitaryBase @base = new(x, y, fraction, starSystemObject);

        foreach (var res in fraction.Resources)
        {
            fraction.Resources[res.Key] = res.Value - Cost.Find(r => r.Key == res.Key).Value;
        }
        Program.Mutex.WaitOne();
        starSystemObject.GroundObjects.Add(@base);
        Program.Mutex.ReleaseMutex();
    }
}
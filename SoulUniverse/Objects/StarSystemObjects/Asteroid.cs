using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.StarSystemObjects;

internal class Asteroid : StarSystemObject
{
    public const int MinSize = 2;
    public const int MaxSize = 5;

    public override double OrbitalSpeed => 107_000;

    public override int Size { get; init; } = Rnd.Next(MinSize, MaxSize);

    protected override char Symbol => '.'; //\u00B7

    public override ConsoleColor Color => ConsoleColor.Gray;

    public Asteroid(int distance) : base(distance)
    {
        //Size = rnd.Next(minSize, maxSize);
        foreach (ResourceName res in Enum.GetValues(typeof(ResourceName)))
        {
            Recources.Add(res, Rnd.Next(100000000));
        }
    }
}
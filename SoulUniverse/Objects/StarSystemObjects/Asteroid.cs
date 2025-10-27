using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.StarSystemObjects;

internal class Asteroid : StarSystemObject
{
    public const int MinSize = 2;
    public const int MaxSize = 5;

    public override double OrbitalSpeed => 107_000;

    public override int Size { get; } = Rnd.Next(MinSize, MaxSize);

    protected override char Symbol => '.'; //\u00B7

    public override ConsoleColor Color => ConsoleColor.Gray;

    public Asteroid(int distance) : base(distance)
    {
        //Size = rnd.Next(minSize, maxSize);
        foreach (ResourceName res in Enum.GetValues(typeof(ResourceName)))
        {
            Resources.Add(res, Rnd.Next(100000000));
        }
    }

    protected override void WriteSelfInfo(ref int row)
    {
        Console.Write($"Астероид {Name}");
        Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
        Console.Write($"Диаметр: {Size * 1000} км");
        Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
        Console.Write($"Расстояние до родительского тела: {OrbitRadius} а.е. ");
        Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
    }
}
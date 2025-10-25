using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

internal class Wormhole : VoidObject
{
    protected override char Symbol { get; } = '@';
    protected override ConsoleColor Color { get; } = ConsoleColor.Gray;

    public Wormhole()
    {
        Coordinates.X = Rnd.Next(Universe.UniverseX);
        Coordinates.Y = Rnd.Next(Universe.UniverseY);
    }

    public Wormhole(int x, int y)
    {
        Coordinates.X = x;
        Coordinates.Y = y;
    }

    public override void Draw()
    {
        if (FractionDisplayMode == DisplayMode.Fractions)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        else Console.ForegroundColor = Color;
        Console.SetCursorPosition(Coordinates.X, Coordinates.Y);
        Console.Write(Symbol);
    }
}
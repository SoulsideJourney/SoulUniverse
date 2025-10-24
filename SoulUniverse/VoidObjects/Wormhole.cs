using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.VoidObjects;

internal class Wormhole : VoidObject
{
    protected override char Symbol { get; } = '@';
    protected override ConsoleColor Color { get; } = ConsoleColor.Gray;

    public Wormhole()
    {
        Coordinates.x = Rnd.Next(Universe.UniverseX);
        Coordinates.y = Rnd.Next(Universe.UniverseY);
    }

    public Wormhole(int x, int y)
    {
        Coordinates.x = x;
        Coordinates.y = y;
    }

    public override void Draw()
    {
        if (FractionDisplayMode == DisplayMode.Fractions)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        else Console.ForegroundColor = Color;
        Console.SetCursorPosition(Coordinates.x, Coordinates.y);
        Console.Write(Symbol);
    }
}
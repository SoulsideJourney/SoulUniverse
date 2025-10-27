using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

internal class Wormhole : VoidObject
{
    protected override ConsoleColor Color => ConsoleColor.Gray;

    protected override char Symbol => '@';

    public override void Draw()
    {
        Console.ForegroundColor = FractionDisplayMode == DisplayMode.Fractions ? ConsoleColor.Gray : Color;
        Console.SetCursorPosition(Coordinates.X, Coordinates.Y);
        Console.Write(Symbol);
    }
}
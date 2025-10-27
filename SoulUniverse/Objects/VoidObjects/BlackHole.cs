using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

internal class BlackHole : VoidObject
{
    protected override char Symbol { get; } = '\u00D8'; //Ø

    protected override ConsoleColor Color { get; } = ConsoleColor.Yellow;

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
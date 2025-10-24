using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

internal class BlackHole : VoidObject
{
    protected override char Symbol { get; } = '\u00D8'; //Ø

    protected override ConsoleColor Color { get; } = ConsoleColor.Yellow;
    public BlackHole()
    {
        Coordinates.X = Rnd.Next(Universe.UniverseX);
        Coordinates.Y = Rnd.Next(Universe.UniverseY);
    }

    public BlackHole(int x, int y)
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
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

internal class BlackHole : VoidObject
{
    protected override char Symbol { get; } = '\u00D8'; //Ø

    protected override ConsoleColor Color { get; } = ConsoleColor.Yellow;

    public BlackHole()
    {
        Coordinates = new Coordinates(Rnd.Next(Universe.UniverseX), Rnd.Next(Universe.UniverseY));
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
        SetCursor(Coordinates);
        Write(Symbol);
    }
}
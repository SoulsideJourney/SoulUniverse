using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.PlanetObjects;

public abstract class GroundProperty : GroundObject
{
    public bool IsNeedToDraw { get; set; } = true;
    public Fraction Owner { get; }

    protected GroundProperty(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, starSystemObject)
    {
        Owner = fraction;
    }

    public override void WriteInfo()
    {
        lock (Locker)
        {
            int row = 2;
            ResetConsoleColor();

            //Общая информация
            Console.SetCursorPosition(Universe.UniverseX + 2, row);
            Console.Write("Информация об объекте: ");
            Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
            Console.Write(Name + " фракции ");
            Console.ForegroundColor = Owner.Color;
            Console.Write(Owner.Name);
            ResetConsoleColor();
            InfoIsClear = false;
            ResetCursor();
        }
    }

    public override void Draw()
    {
        lock (Locker)
        {
            Console.SetCursorPosition(Coordinates.X, Coordinates.Y);
            if (FractionDisplayMode == Enums.DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
            else ResetConsoleColor();
            Console.Write(Symbol);
            ResetConsoleColor();
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            IsNeedToDraw = false;
        }
    }
}
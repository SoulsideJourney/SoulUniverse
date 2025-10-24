using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.PlanetObjects;

public abstract class GroundProperty : GroundObject
{
    public bool IsNeedToDraw { get; set; } = true;
    public Fraction Owner { get; }
    //static public abstract List<KeyValuePair<ResourceName, int>> Cost { get; }
    //public abstract void Draw();


    protected GroundProperty(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, starSystemObject)
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
            Console.SetCursorPosition(Coordinates.x, Coordinates.y);
            if (FractionDisplayMode == Enums.DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
            else ResetConsoleColor();
            Console.Write(Symbol);
            ResetConsoleColor();
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            IsNeedToDraw = false;
        }
    }
}
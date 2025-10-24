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
            SetCursor(Universe.UniverseX + 2, row);
            Write("Информация об объекте: ");
            SetCursor(Universe.UniverseX + 2, ++row);
            Write(Name + " фракции ");
            Console.ForegroundColor = Owner.Color;
            Write(Owner.Name);
            ResetConsoleColor();
            InfoIsClear = false;
            ResetCursor();
        }
    }

    public override void Draw()
    {
        lock (Locker)
        {
            SetCursor(Coordinates.X, Coordinates.Y);
            if (FractionDisplayMode == Enums.DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
            else ResetConsoleColor();
            Write(Symbol);
            ResetConsoleColor();
            SetCursor(CurrentCursorX, CurrentCursorY);
            IsNeedToDraw = false;
        }
    }
}
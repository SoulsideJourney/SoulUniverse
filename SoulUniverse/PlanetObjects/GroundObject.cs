using SoulUniverse.StarSystemObjects;
using static SoulUniverse.Program;

namespace SoulUniverse.PlanetObjects;

public abstract class GroundObject : UniverseObject
{
    protected abstract char Symbol { get; }
    protected abstract string Name { get; }
    public StarSystemObject Location { get; }
    public Coordinates Coordinates { get; } = new();

    protected GroundObject(int x, int y, StarSystemObject starSystemObject)
    {
        Coordinates.x = x;
        Coordinates.y = y;
        Location = starSystemObject;
    }

    public virtual void WriteInfo()
    {
        lock(Locker)
        {
            int row = 2;
            ResetConsoleColor();

            //Общая информация
            Console.SetCursorPosition(Universe.UniverseX + 2, row);
            Console.Write("Информация об объекте: ");
            Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
            Console.Write(Name);
            InfoIsClear = false;
            ResetCursor();
        }
    }

    public override void Draw()
    {
        lock(Locker)
        {
            Console.SetCursorPosition(Coordinates.x, Coordinates.y);
            //if (FractionDisplayMode == Enums.DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
            //else ResetConsoleColor();
            Console.Write(Symbol);
            ResetConsoleColor();
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            //IsNeedToDraw = false;
        }  
    }
}
using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.PlanetObjects;

public abstract class GroundObject : UniverseObject
{
    protected abstract char Symbol { get; }
    protected abstract string Name { get; }
    public StarSystemObject Location { get; }
    public Coordinates Coordinates { get; protected set; }

    protected GroundObject(Coordinates coordinates, StarSystemObject starSystemObject)
    {
        Coordinates = coordinates;
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
            Console.SetCursorPosition(Coordinates.X, Coordinates.Y);
            //if (FractionDisplayMode == Enums.DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
            //else ResetConsoleColor();
            Console.Write(Symbol);
            ResetConsoleColor();
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            //IsNeedToDraw = false;
        }  
    }
}
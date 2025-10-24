using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

internal abstract class VoidObject : UniverseObject
{
    protected abstract char Symbol { get; }

    protected abstract ConsoleColor Color { get; }

    public Coordinates Coordinates = new();
    //public abstract void Draw();

    public void WriteInfo()
    {
        int row = 2;
        ResetConsoleColor();
        SetCursor(Universe.UniverseX + 2, row);
        Write("Информация об объекте: ");
        if (this is Star star)
        {
            star.WriteStarInfo();
        }
        else if (this is BlackHole)
        {
            Write("Черная дыра");
        }
        else if (this is Wormhole)
        {
            Write("Червоточина");
        }
        else Write("Неизвестный объект");
        InfoIsClear = false;
        CheckedVoidObject = this;

        //Возвращение курсора
        SetCursor(CurrentCursorX, CurrentCursorY);
    }
}
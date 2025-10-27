using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

/// <summary> Небесное тело в галактике (звездного масштаба) </summary>
public abstract class VoidObject : UniverseObject
{
    protected abstract char Symbol { get; }

    protected abstract ConsoleColor Color { get; }

    public Coordinates Coordinates = new();

    public void WriteInfo()
    {
        int row = 2;
        ResetConsoleColor();
        Console.SetCursorPosition(Universe.UniverseX + 2, row);
        Console.Write("Информация об объекте: ");
        if (this is Star star)
        {
            star.WriteStarInfo();
        }
        else if (this is BlackHole)
        {
            Console.Write("Черная дыра");
        }
        else if (this is Wormhole)
        {
            Console.Write("Червоточина");
        }
        else Console.Write("Неизвестный объект");
        InfoIsClear = false;
        CheckedVoidObject = this;

        //Возвращение курсора
        Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
    }
}
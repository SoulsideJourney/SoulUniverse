using SoulUniverse.Interfaces;
using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.PlanetObjects;

/// <summary> Чья-то собственность </summary>
public abstract class GroundProperty : GroundObject, IDestructible
{
    public abstract int Health { get; set; }

    /// <summary> Не помню, зачем это вообще было. Возможно, подразумевалось, что не все объекты будут рисоваться </summary>
    public bool IsNeedToDraw { get; private set; } = true;

    /// <summary> Фракция-владелец объекта </summary>
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

    public void TakeDamage(int receivedDamage)
    {
        Health -= receivedDamage;
        if (Health <= 0) DestroySelf();
    }

    public virtual void DestroySelf()
    {
        // TODO подумоть

        //Затирать только если он вообще отрисован (режим просмотра планеты)
        throw new NotImplementedException();
    }
}
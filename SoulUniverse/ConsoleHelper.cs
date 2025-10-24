using static SoulUniverse.Program;

namespace SoulUniverse;

internal static class ConsoleHelper
{
    /// <summary> Размер консоли по оси X </summary>
    public const int ConsoleX = 200;

    /// <summary> Размер консоли по оси Y </summary>
    public const int ConsoleY = 50;

    private static int _currentCursorX;
    private static int _currentCursorY;

    public static int CurrentCursorX
    {
        get => _currentCursorX;
        set
        {
            _currentCursorX = value;
            SetConsoleTitle();
        }
    }

    public static int CurrentCursorY
    {
        get => _currentCursorY;
        set
        {
            _currentCursorY = value;
            SetConsoleTitle();
        }
    }

    public static void SetCursor(Coordinates coordinates) => SetCursor(coordinates.X, coordinates.Y);

    /// <summary> Установить курсор </summary>
    public static void SetCursor(int x, int y)
    {
        CurrentCursorX = x;
        CurrentCursorY = y;
        Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
    }

    public static void ResetConsoleColor()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void Write(string? value) => Console.Write(value);

    public static void Write(char? value) => Console.Write(value);

    public static void Write(object? value) => Console.Write(value);
}
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

    /// <summary> Установить курсор </summary>
    public static void SetCursor(int x, int y)
    {
        CurrentCursorX = x;
        CurrentCursorY = y;
        Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
    }
}
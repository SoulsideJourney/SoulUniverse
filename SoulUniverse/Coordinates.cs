namespace SoulUniverse;

public struct Coordinates(int x, int y)
{
    public int X = x;
    public int Y = y;

    /// <summary> Разница координатах по горизонтали и вертикали 1 </summary>
    public bool IsNearby(Coordinates other)
    {
        return X - other.X is 1 or -1 && Y - other.Y is 1 or -1;
    }

    /// <summary> Координаты близки </summary>
    public bool IsWithin(Coordinates other, int distance)
    {
        return (X - other.X < distance || X - other.X > -distance) &&
               (Y - other.Y < distance || Y - other.Y > -distance);
    }
}
namespace SoulUniverse;

/// <summary> Движущиеся объекты </summary>
public interface IMovable
{
    bool IsNeedToRedraw { get; set; }
    Coordinates DrawnCoordinates { get; set; }
    void Move();
    void Erase();
}
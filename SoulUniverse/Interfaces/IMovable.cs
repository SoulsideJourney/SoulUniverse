namespace SoulUniverse.Interfaces;

/// <summary> Движущиеся объекты </summary>
public interface IMovable
{
    /// <summary> Объект переместился и нуждается в перерисовке в консоли </summary>
    bool IsNeedToRedraw { get; set; }

    /// <summary> Координаты, в которых отрисован объект. Могут не совпадать с расчетными </summary>
    Coordinates DrawnCoordinates { get; set; }

    void Move();

    void Erase();
}
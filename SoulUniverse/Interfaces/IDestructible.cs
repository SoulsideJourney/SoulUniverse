namespace SoulUniverse.Interfaces;

internal interface IDestructible
{
    /// <summary> Здоровье </summary>
    public int Health { get; set; }

    /// <summary> Получение урона </summary>
    public void TakeDamage(int receivedDamage);

    /// <summary> Выпиливание данного объекта отовсюду при его уничтожении </summary>
    public void DestroySelf();
}
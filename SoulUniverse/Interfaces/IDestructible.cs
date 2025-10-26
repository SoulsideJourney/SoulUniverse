namespace SoulUniverse.Interfaces;

internal interface IDestructible
{
    /// <summary> Здоровье </summary>
    public int Health { get; set; }

    /// <summary> Получение урона </summary>
    public void Damage(int receivedDamage);

    /// <summary> Уничтожение </summary>
    public void Destroy();
}
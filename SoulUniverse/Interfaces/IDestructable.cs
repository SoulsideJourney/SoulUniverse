namespace SoulUniverse.Interfaces;

internal interface IDestructable
{
    public int Health { get; set; }

    public void Destroy();
}
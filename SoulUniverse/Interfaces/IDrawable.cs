namespace SoulUniverse.Interfaces;

internal interface IDrawable
{
    public Coordinates Coordinates { get; }

    public void Draw();

    public void WriteInfo();
}
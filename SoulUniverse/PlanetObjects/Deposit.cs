using SoulUniverse.StarSystemObjects;

namespace SoulUniverse.PlanetObjects;

public class Deposit : GroundObject
{
    public Deposit(int x, int y, StarSystemObject starSystemObject, Enums.ResourceName resource) : base(x, y, starSystemObject)
    {
        Resource = resource;
    }

    public Mine? Mine { get; set; }

    public Enums.ResourceName Resource { get; init; }

    /// <summary> Занято ли месторождение шахтой </summary>
    public bool IsOccupied => Mine is not null;

    protected override char Symbol => Resource switch
    {
        Enums.ResourceName.Iron => '\u25b2', //▲
        Enums.ResourceName.Oil => '\u25b3', //△
        _ => '\u25d9' //◙
    }; //▲

    protected override string Name => $"Месторождение {Resource}";

    public override void Draw()
    {
        if (!IsOccupied)
        {
            base.Draw();
        }
    }

    public override void WriteInfo()
    {
        if (!IsOccupied)
        {
            base.WriteInfo();
        }
    }
}
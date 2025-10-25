using SoulUniverse.Interfaces;
using static SoulUniverse.Enums;

namespace SoulUniverse.Objects.PlanetObjects;

/// <summary> Шахта </summary>
public class Mine : GroundProperty, IBuildable
{
    protected override char Symbol => '^';
    protected override string Name => $"Шахта ({Deposit.Resource})";

    private Deposit _deposit;

    /// <summary> Занимаемый шахтой депозит </summary>
    public Deposit Deposit
    {
        get => _deposit;
        set
        {
            _deposit = value;
            _deposit.Mine = this;
        }
    }

    public static List<KeyValuePair<ResourceName, int>> BuildCost { get; } =
    [
        new(ResourceName.Iron, 100),
        new(ResourceName.Uranium, 0),
        new(ResourceName.Oil, 0)
    ];

    public Mine(Fraction fraction, Deposit deposit) : base(deposit.Coordinates, fraction, deposit.Location)
    {
        foreach (var res in fraction.Resources)
        {
            fraction.Resources[res.Key] = res.Value - BuildCost.Find(r => r.Key == res.Key).Value;
        }

        Deposit = deposit;
    }

    /// <summary> Добывать ресурсы </summary>
    public void Excavate()
    {
        if (Location.Recources[Deposit.Resource] > 0)
        {
            Location.Recources[Deposit.Resource] -= 1;
            Owner.Resources[Deposit.Resource] += 1;
        }
    }
}
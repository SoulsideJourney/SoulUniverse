using SoulUniverse.StarSystemObjects;
using static SoulUniverse.Enums;

namespace SoulUniverse.PlanetObjects;

internal class Factory : GroundProperty
{
    protected override char Symbol => 'F';

    protected override string Name => "Завод";

    public static List<KeyValuePair<ResourceName, int>> Cost { get; } =
    [
        new(ResourceName.Iron, 1500),
        new(ResourceName.Uranium, 0),
        new(ResourceName.Oil, 0),
    ];

    private Factory(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject) { }

    /// <summary> Создать фабрику </summary>
    //Фабрика фабрик)))
    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        Factory factory = new(x, y, fraction, starSystemObject);
        Universe.Factories.Add(factory);
        starSystemObject.GroundObjects.Add(factory);
    }

    public void BuildTank()
    {
        Tank.New(Coordinates.x + 1, Coordinates.y, Owner, Location);
    }
}
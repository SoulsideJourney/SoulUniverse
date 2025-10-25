using SoulUniverse.Objects.StarSystemObjects;
using System.Diagnostics;
using static SoulUniverse.Enums;

namespace SoulUniverse.Objects.PlanetObjects;

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

    private Factory(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, fraction, starSystemObject) { }

    /// <summary> Создать фабрику </summary>
    //Фабрика фабрик)))
    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        Factory factory = new(new Coordinates(x, y), fraction, starSystemObject);
        Universe.Factories.Add(factory);
        starSystemObject.GroundObjects.Add(factory);
    }

    public void BuildTank()
    {
        Tank.New(Coordinates.X + 1, Coordinates.Y, Owner, Location);
        Debug.WriteLine(string.Format($"Насекомые из {Owner.Name} построили ТАНК! Будет война"));
    }
}
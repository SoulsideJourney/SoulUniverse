using SoulUniverse.Objects.StarSystemObjects;
using System.Diagnostics;
using static SoulUniverse.Enums;

namespace SoulUniverse.Objects.PlanetObjects;

internal class Factory : GroundProperty
{
    public static List<KeyValuePair<ResourceName, int>> BuildCost { get; } =
    [
        new(ResourceName.Iron, 1500),
        new(ResourceName.Uranium, 0),
        new(ResourceName.Oil, 0),
    ];

    public override int Health { get; set; } = 1000;

    protected override string Name => "Завод";

    protected override char Symbol => 'F';

    private Factory(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, fraction, starSystemObject) { }

    /// <summary> Создать фабрику </summary>
    //Фабрика фабрик)))
    public static void New(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
    {
        Factory factory = new(new Coordinates(x, y), fraction, starSystemObject);
        Universe.Factories.Add(factory);
        starSystemObject.GroundObjects.Add(factory);
    }

    /// <summary> Фабрика будет пытаться построить танк </summary>
    public bool TryBuildTank()
    {
        if (Owner.IsEnoughToBuildTank()
            && Location.GroundObjects.Count(o => o is Tank tank && tank.Owner == Owner) < Location.PlacesCount * 0.8
            && !Location.IsPlaceOccupied(Coordinates.X + 1, Coordinates.Y))
        {
            BuildTank();
            return true;
        }

        return false;
    }

    public void BuildTank()
    {
        Tank.New(Coordinates.X + 1, Coordinates.Y, Owner, Location);
        Debug.WriteLine(string.Format($"Насекомые из {Owner.Name} построили ТАНК! Будет война"));
    }

    public override void DestroySelf()
    {
        Universe.Factories.Remove(this);
        base.DestroySelf();
    }
}
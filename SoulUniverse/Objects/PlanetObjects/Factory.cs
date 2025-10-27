using SoulUniverse.Objects.StarSystemObjects;
using System.Diagnostics;
using static SoulUniverse.Enums;

namespace SoulUniverse.Objects.PlanetObjects;

/// <summary> Завод для работяг </summary>
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

    public void Work()
    {
        var hostileCount = Location.GroundObjects.Count(o => o is GroundProperty p && p.Owner != Owner);
        var tanksCount = Location.GroundObjects.Count(o => o is Tank t && t.Owner == Owner);
        var shipsCount = Location.GroundObjects.Count(o => o is ColonialShip s && s.Owner == Owner);

        //Если нет вражеских объектов, соотношение танков и кораблей больше 40:1
        if (hostileCount == 0 && tanksCount > 40 && (shipsCount == 0 || tanksCount / shipsCount > 20))
        {
            TryBuildColonialShip();
        }
        else TryBuildTank();
    }

    private bool TryBuildColonialShip()
    {
        if (Owner.IsEnoughToBuildColonialShip()
            && Location.GroundObjects.Count(o => o is GroundProperty p && p.Owner == Owner) < Location.PlacesCount * 0.8
            && !Location.IsPlaceOccupied(Coordinates.X + 1, Coordinates.Y))
        {
            BuildColonialShip();
            return true;
        }

        return false;
    }

    private void BuildColonialShip()
    {
        ColonialShip.New(Coordinates.X + 1, Coordinates.Y, Owner, Location);
        Debug.WriteLine(string.Format($"Насекомые из {Owner.Name} построили КОЛОНИАЛЬНЫЙ КОРАБЛЬ! Будет захват галактики"));
    }

    /// <summary> Фабрика будет пытаться построить танк </summary>
    private bool TryBuildTank()
    {
        if (Owner.IsEnoughToBuildTank()
            && Location.GroundObjects.Count(o => o is GroundProperty p && p.Owner == Owner) < Location.PlacesCount * 0.8
            && !Location.IsPlaceOccupied(Coordinates.X + 1, Coordinates.Y))
        {
            BuildTank();
            return true;
        }

        return false;
    }

    private void BuildTank()
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
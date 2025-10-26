using SoulUniverse.Interfaces;
using SoulUniverse.Objects.StarSystemObjects;

namespace SoulUniverse.Objects.PlanetObjects;

/// <summary> Колониальный корабль </summary>
internal class ColonialShip : GroundProperty, IBuildable
{
    public ColonialShip(Coordinates coordinates, Fraction fraction, StarSystemObject starSystemObject) : base(coordinates, fraction, starSystemObject) { }

    public override int Health { get; set; } = 1000;

    protected override string Name => "Колониальный корабль";

    protected override char Symbol => '◊';
}
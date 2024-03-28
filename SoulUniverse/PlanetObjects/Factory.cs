using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.PlanetObjects
{
    internal class Factory : GroundObject
    {
        protected override char Symbol => 'F';

        protected override string Name => "Завод";
        static public List<KeyValuePair<ResourceName, int>> Cost { get; } = new()
        {
            new KeyValuePair<ResourceName, int>(ResourceName.Iron, 1500),
            new KeyValuePair<ResourceName, int>(ResourceName.Uranium, 0),
            new KeyValuePair<ResourceName, int>(ResourceName.Oil, 0),
        };

        public Factory(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject)
        {
            Universe.Factories.Add(this);
            starSystemObject.GroundObjects.Add(this);
        }

        public void BuildTank()
        {
            //new Tank(rnd.Next(starSystemObject.Size), rnd.Next(starSystemObject.Size), this.Owner, starSystemObject);
            new Tank(Coordinates.x + 1, Coordinates.y, Owner, Location);
        }
    }
}

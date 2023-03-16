using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;

namespace SoulUniverse.PlanetObjects
{
    internal class Tank : GroundObject
    {
        public override Fraction Owner { get; }

        public override StarSystemObject Location { get; }

        protected override char Symbol => 'T';

        protected override string Name => "Танк мать его";

        static public List<KeyValuePair<ResourceName, int>> Cost { get; } = new()
        {
            new KeyValuePair<ResourceName, int>(ResourceName.Iron, 10),
            new KeyValuePair<ResourceName, int>(ResourceName.Uranium, 1),
            new KeyValuePair<ResourceName, int>(ResourceName.Oil, 5),
        };

        public Tank(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
        {
            Coordinates.x = x;
            Coordinates.y = y;
            Owner = fraction;
            Location = starSystemObject;
            //Program.mines.Add(this);
        }
    }
}

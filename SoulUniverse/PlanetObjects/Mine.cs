using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;

namespace SoulUniverse.PlanetObjects
{
    internal class Mine : GroundObject
    {
        protected override char Symbol { get; } = '^';
        protected override string Name { get; } = "Шахта";
        public override Fraction Owner { get; }
        public override StarSystemObject Location { get; }

        static public List<KeyValuePair<ResourceName, int>> Cost { get; } = new()
        {
            new KeyValuePair<ResourceName, int>(ResourceName.Iron, 100),
            new KeyValuePair<ResourceName, int>(ResourceName.Uranium, 0),
            new KeyValuePair<ResourceName, int>(ResourceName.Oil, 0),
        };

        public Mine(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
        {
            Coordinates.x = x;
            Coordinates.y = y;
            Owner = fraction;
            Location = starSystemObject;
            Program.mines.Add(this);
        }

        public void Excavate()
        {
            Location.Recources[ResourceName.Iron] -= 1;
            Owner.Recources[ResourceName.Iron] += 1;
            //Debug.WriteLine("Шахта {0} работает", this.ToString());
        }
    }
}

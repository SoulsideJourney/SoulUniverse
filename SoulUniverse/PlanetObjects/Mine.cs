using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.PlanetObjects
{
    internal class Mine : GroundProperty/*, IBuildable*/
    {
        protected override char Symbol => '^';
        protected override string Name => "Шахта";

        public static List<KeyValuePair<ResourceName, int>> BuildCost { get; } =
        [
            new KeyValuePair<ResourceName, int>(ResourceName.Iron, 100),
            new KeyValuePair<ResourceName, int>(ResourceName.Uranium, 0),
            new KeyValuePair<ResourceName, int>(ResourceName.Oil, 0)
        ];

        public Mine(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject)
        {
            foreach (var res in fraction.Recources)
            {
                fraction.Recources[res.Key] = res.Value - BuildCost.Find(r => r.Key == res.Key).Value;
            }
        }

        public Mine(Fraction fraction, Deposit deposit) : base(deposit.Coordinates.x, deposit.Coordinates.y, fraction, deposit.Location)
        {
            foreach (var res in fraction.Recources)
            {
                fraction.Recources[res.Key] = res.Value - BuildCost.Find(r => r.Key == res.Key).Value;
            }

            deposit.IsOccupied = true;
        }

        public void Excavate()
        {
            if (Location.Recources[ResourceName.Iron] > 0)
            {
                Location.Recources[ResourceName.Iron] -= 1;
                Owner.Recources[ResourceName.Iron] += 1;
            }
        }
    }
}

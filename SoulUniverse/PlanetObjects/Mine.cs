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
    internal class Mine : GroundObject//, IImmovable
    {
        protected override char Symbol { get; } = '^';
        protected override string Name { get; } = "Шахта";

        static public List<KeyValuePair<ResourceName, int>> Cost { get; } = new()
        {
            new KeyValuePair<ResourceName, int>(ResourceName.Iron, 100),
            new KeyValuePair<ResourceName, int>(ResourceName.Uranium, 0),
            new KeyValuePair<ResourceName, int>(ResourceName.Oil, 0),
        };

        //public bool IsNeedToDraw { get; set; } = true;

        public Mine(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject)
        {
            foreach (var res in fraction.Recources)
            {
                fraction.Recources[res.Key] = res.Value - Mine.Cost.Find(r => r.Key == res.Key).Value;
            }

            mutex.WaitOne();
            Program.mines.Add(this);
            starSystemObject.GroundObjects.Add(this);
            mutex.ReleaseMutex();
        }

        public void Excavate()
        {
            Location.Recources[ResourceName.Iron] -= 1;
            Owner.Recources[ResourceName.Iron] += 1;
        }
    }
}

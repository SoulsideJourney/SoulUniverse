using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.PlanetObjects
{
    internal class MilitaryBase : GroundObject//, IImmovable
    {
        protected override char Symbol => '#';
        protected override string Name => "Военная база";
        //public override Fraction Owner { get; }
        //public override StarSystemObject Location { get; }
        static public List<KeyValuePair<ResourceName, int>> Cost { get; } = new()
        {
            new KeyValuePair<ResourceName, int>(ResourceName.Iron, 1000),
            new KeyValuePair<ResourceName, int>(ResourceName.Uranium, 0),
            new KeyValuePair<ResourceName, int>(ResourceName.Oil, 0),
        };
        //public bool IsNeedToDraw { get; set; } = true;

        public MilitaryBase(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject)
        {
            foreach (var res in fraction.Recources)
            {
                fraction.Recources[res.Key] = res.Value - Cost.Find(r => r.Key == res.Key).Value;
            }
            mutex.WaitOne();
            starSystemObject.GroundObjects.Add(this);
            mutex.ReleaseMutex();
        }
        //public MilitaryBase(int x, int y, Fraction fraction)
        //{
        //    Coordinates.x = x;
        //    Coordinates.y = y;
        //    Owner = fraction;
        //}
    }
}

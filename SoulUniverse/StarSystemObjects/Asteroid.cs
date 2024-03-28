using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.StarSystemObjects
{
    internal class Asteroid : StarSystemObject
    {
        public const int minSize = 2;
        public const int maxSize = 5;

        public override double OrbitalSpeed => 107_000;

        public override int Size { get; init; } = Rnd.Next(minSize, maxSize);

        protected override char Symbol => '.'; //\u00B7

        public override ConsoleColor Color => ConsoleColor.Gray;

        public Asteroid(int distance) : base(distance)
        {
            //Size = rnd.Next(minSize, maxSize);
            foreach (ResourceName res in Enum.GetValues(typeof(ResourceName)))
            {
                Recources.Add(res, Rnd.Next(100000000));
            }
        }
    }
}

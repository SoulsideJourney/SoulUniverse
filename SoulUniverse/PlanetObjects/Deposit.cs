using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulUniverse.PlanetObjects
{
    internal class Deposit : GroundObject
    {
        public Deposit(int x, int y, StarSystemObject starSystemObject) : base(x, y, starSystemObject)
        {
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        protected override char Symbol => '\u25b2'; //▲
        protected override string Name => "Месторождение ЖЫЛЕЗА";
    }
}

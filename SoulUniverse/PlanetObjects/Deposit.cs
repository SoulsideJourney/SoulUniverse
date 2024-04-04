using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulUniverse.PlanetObjects
{
    public class Deposit : GroundObject
    {
        public Deposit(int x, int y, StarSystemObject starSystemObject) : base(x, y, starSystemObject)
        {

        }

        /// <summary> Занято ли месторождение шахтой </summary>
        public bool IsOccupied { get; set; }

        protected override char Symbol => '\u25b2'; //▲
        protected override string Name => "Месторождение ЖЫЛЕЗА";

        public override void Draw()
        {
            if (!IsOccupied)
            {
                base.Draw();
            }
        }

        public override void WriteInfo()
        {
            if (!IsOccupied)
            {
                base.WriteInfo();
            }
        }
    }
}

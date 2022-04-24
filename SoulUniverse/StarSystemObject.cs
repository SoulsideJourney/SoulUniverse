using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulUniverse
{
    internal abstract class StarSystemObject
    {
        public int Distance { get; protected set;  }
        public Coordinates Coordinates = new();

        public abstract void Draw();
    }
}

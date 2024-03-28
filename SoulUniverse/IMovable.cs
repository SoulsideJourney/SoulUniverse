using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulUniverse
{
    public interface IMovable
    {
        bool IsNeedToRedraw { get; set; }
        Coordinates DrawedCoordinates { get; set; }
        void Move();
        void Erase();
    }
}

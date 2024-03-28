using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulUniverse
{
    /// <summary> Движущиеся объекты </summary>
    public interface IMovable
    {
        bool IsNeedToRedraw { get; set; }
        Coordinates DrawedCoordinates { get; set; }
        void Move();
        void Erase();
    }
}

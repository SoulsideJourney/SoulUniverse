using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Enums;

namespace SoulUniverse
{
    internal class Resource
    {
        public ResourceName Name { get; }

        public Resource(ResourceName name)
        {
            Name = name;
        }
    }
}

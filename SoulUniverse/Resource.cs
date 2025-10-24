using static SoulUniverse.Enums;

namespace SoulUniverse;

internal class Resource
{
    public ResourceName Name { get; }

    public Resource(ResourceName name)
    {
        Name = name;
    }
}
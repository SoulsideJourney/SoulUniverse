using static SoulUniverse.Enums;

namespace SoulUniverse.Interfaces;

internal interface IBuildable
{
    public static List<KeyValuePair<ResourceName, int>> BuildCost => null!;
}
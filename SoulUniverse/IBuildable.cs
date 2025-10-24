using static SoulUniverse.Enums;

namespace SoulUniverse;

internal interface IBuildable
{
    public static List<KeyValuePair<ResourceName, int>> BuildCost { get; } = null!;
}
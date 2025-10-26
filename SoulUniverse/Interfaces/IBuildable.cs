using static SoulUniverse.Enums;

namespace SoulUniverse.Interfaces;

public interface IBuildable
{
    /// <summary> Стоимость постройки </summary>
    public static List<KeyValuePair<ResourceName, int>> BuildCost => null!;
}
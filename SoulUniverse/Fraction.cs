using SoulUniverse.Objects.PlanetObjects;
using SoulUniverse.Objects.StarSystemObjects;
using System.Diagnostics;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse;

/// <summary> Фракция </summary>
public class Fraction
{
    private Fraction(string name, ConsoleColor color)
    {
        Name = name;
        Color = color;
    }

    public Fraction(FractionName fraction)
    {
        Name = fraction.ToString();
        //Рандомный цвет фракции
        Color = (ConsoleColor)Rnd.Next(Enum.GetValues(typeof(FractionColor)).Length);
        foreach (ResourceName resource in Enum.GetValues(typeof(ResourceName)))
        {
            Resources.Add(resource, 1000);
        }
    }

    public string Name { get; }

    public ConsoleColor Color { get; }

    public List<GroundProperty> Property => Colonies
        .SelectMany(o => o.GroundObjects)
        .Where(go => go is GroundProperty property && property.Owner == this).Cast<GroundProperty>().ToList();

    public List<StarSystemObject> Colonies { get; } = [];

    /// <summary> Все ресурсы фракции </summary>
    public Dictionary<ResourceName, int> Resources { get; } = new();

    /// <summary> Фракция игрока </summary>
    public static Fraction CreatePlayerFraction()
    {
        return new Fraction("Игрок", ConsoleColor.Green);
    }

    /// <summary>NPC-фракция что-нибудь делает</summary>
    public void DoSomething()
    {
        var action = Rnd.Next(2);

        bool result = action switch
        {
            0 => TryBuildMine(),
            1 => TryBuildFactory(),
            _ => TryBuildMilitaryBase()
        };

        if (!result) Debug.WriteLine($"Насекомые из {Name} обнищали");
    }

    /// <summary>Достаточно ли ресурсов на шахту</summary>
    private bool IsEnoughToBuildMine()
    {
        foreach (var kvp in Resources)
        {
            if (kvp.Value < Mine.BuildCost.Find(k => k.Key == kvp.Key).Value)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Достаточно ли ресурсов на военную базу</summary>
    private bool IsEnoughToBuildMilitaryBase()
    {
        foreach (var kvp in Resources)
        {
            if (kvp.Value < MilitaryBase.BuildCost.Find(k => k.Key == kvp.Key).Value)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Достаточно ли ресурсов на танк</summary>
    public bool IsEnoughToBuildTank()
    {
        foreach (var kvp in Resources)
        {
            if (kvp.Value < Tank.Cost.Find(k => k.Key == kvp.Key).Value)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Достаточно ли ресурсов на завод</summary>
    public bool IsEnoughToBuildFactory()
    {
        foreach (var kvp in Resources)
        {
            if (kvp.Value < Factory.Cost.Find(k => k.Key == kvp.Key).Value)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary> Фракция будет пытаться построить военную базу </summary>
    public bool TryBuildMilitaryBase() => TryBuildMilitaryBase(GetRandomOccupiedPlanet());

    private bool TryBuildMilitaryBase(StarSystemObject starSystemObject)
    {
        if (!IsEnoughToBuildMilitaryBase()) return false;

        int x = Rnd.Next(starSystemObject.Size);
        int y = Rnd.Next(starSystemObject.Size);
        if (!starSystemObject.IsPlaceOccupied(x, y))
        {
            MilitaryBase.New(x, y, this, starSystemObject);

            Debug.WriteLine($"Насекомые из {Name} построили базу");
            return true;
        }
        return false;
    }

    /// <summary> Фракция будет пытаться построить шахту </summary>
    public bool TryBuildMine() => TryBuildMine(GetRandomOccupiedPlanet());

    private bool TryBuildMine(StarSystemObject starSystemObject)
    {
        if (!IsEnoughToBuildMine()) return false;
        var deposit = starSystemObject.Deposits.FirstOrDefault(d => !d.IsOccupied);
        if (deposit != null)
        {
            Mine mine = new(this, deposit);

            Program.Mutex.WaitOne();
            Universe.Mines.Add(mine);
            starSystemObject.GroundObjects.Add(mine);
            Program.Mutex.ReleaseMutex();

            Debug.WriteLine($"Насекомые из {Name} построили шахту");
            return true;
        }
        return false;
    }

    /// <summary> Фракция будет пытаться построить завод </summary>
    public bool TryBuildFactory() => TryBuildFactory(GetRandomOccupiedPlanet());

    private bool TryBuildFactory(StarSystemObject starSystemObject)
    {
        if (!IsEnoughToBuildFactory()) return false;
        int x = Rnd.Next(starSystemObject.Size);
        int y = Rnd.Next(starSystemObject.Size);
        if (!starSystemObject.IsPlaceOccupied(x, y))
        {
            Factory.New(x, y, this, starSystemObject);

            Debug.WriteLine($"Насекомые из {Name} построили завод! Работягам будет, где работать");
            return true;
        }
        return false;
    }

    private StarSystemObject GetRandomOccupiedPlanet() => Colonies.ElementAt(Rnd.Next(Colonies.Count));
}
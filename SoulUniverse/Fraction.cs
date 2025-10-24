using SoulUniverse.PlanetObjects;
using SoulUniverse.StarSystemObjects;
using System.Diagnostics;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse;

public class Fraction
{
    public string Name { get; protected set; }

    public ConsoleColor Color { get; protected set; }
    //public FractionColor Color { get; protected set; }
    //public List<GroundObject> Property { get; } = new();
    public List<GroundProperty> Property => Colonies
        .SelectMany(o => o.GroundObjects)
        .Where(go => go is GroundProperty property && property.Owner == this).Cast<GroundProperty>().ToList();

    public List<StarSystemObject> Colonies { get; protected set; } = new();
    public Dictionary<ResourceName, int> Recources { get; } = new();

    //Фракция игрока
    public Fraction()
    {
        Name = "Игрок";
        Color = ConsoleColor.Green;
    }

    public Fraction(FractionName fraction)
    {
        Name = fraction.ToString();
        Random rnd = new();
        //Рандомный цвет фракции
        //Color = (ConsoleColor)Enum.GetValues(typeof(FractionColor)).GetValue(rnd.Next(Enum.GetValues(typeof(FractionColor)).Length));
        Color = (ConsoleColor)rnd.Next(Enum.GetValues(typeof(FractionColor)).Length);
        foreach (ResourceName resource in Enum.GetValues(typeof(ResourceName)))
        {
            //Recources.Add(new KeyValuePair<ResourceName, int>(resource, 1000));
            Recources.Add(resource, 1000);
        }
    }

    /// <summary>Фракция что-нибудь делает</summary>
    public void DoSomething()
    {
        if (IsEnoughToBuildMine())
        {
            if (TryBuildMine(this.Colonies.ElementAt(Rnd.Next(Colonies.Count))))
            {
                Debug.WriteLine($"Насекомые из {Name} построили шахту");
            }

            if (IsEnoughToBuildFactory())
            {
                if (TryBuildFactory(this.Colonies.ElementAt(Rnd.Next(Colonies.Count))))
                {
                    Debug.WriteLine($"Насекомые из {Name} построили завод! Работягам будет, где работать");
                }
            }

            if (IsEnoughToBuildMilitaryBase())
            {
                if (TryBuildMilitaryBase(this.Colonies.ElementAt(Rnd.Next(Colonies.Count))))
                {
                    Debug.WriteLine($"Насекомые из {Name} построили базу");
                }
            }
        }

        else Debug.WriteLine($"Насекомые из {Name} обнищали");
    }

    /// <summary>Достаточно ли ресурсов на шахту</summary>
    public bool IsEnoughToBuildMine()
    {
        foreach (var kvp in Recources)
        {
            if (kvp.Value < Mine.BuildCost.Find(k => k.Key == kvp.Key).Value)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Достаточно ли ресурсов на военную базу</summary>
    public bool IsEnoughToBuildMilitaryBase()
    {
        foreach (var kvp in Recources)
        {
            if (kvp.Value < MilitaryBase.Cost.Find(k => k.Key == kvp.Key).Value)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>Достаточно ли ресурсов на танк</summary>
    public bool IsEnoughToBuildTank()
    {
        foreach (var kvp in Recources)
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
        foreach (var kvp in Recources)
        {
            if (kvp.Value < Factory.Cost.Find(k => k.Key == kvp.Key).Value)
            {
                return false;
            }
        }
        return true;
    }

    //public void BuildMilitaryBase(StarSystemObject starSystemObject)
    //{
    //    new MilitaryBase(rnd.Next(starSystemObject.Size), rnd.Next(starSystemObject.Size), this, starSystemObject);
    //}

    public bool TryBuildMilitaryBase(StarSystemObject starSystemObject)
    {
        int x = Rnd.Next(starSystemObject.Size);
        int y = Rnd.Next(starSystemObject.Size);
        if (!starSystemObject.IsPlaceOccupied(x, y))
        {
            MilitaryBase.New(x, y, this, starSystemObject);
            return true;
        }
        return false;
    }

    //public void BuildMine(StarSystemObject starSystemObject)
    //{
    //    new Mine(rnd.Next(starSystemObject.Size), rnd.Next(starSystemObject.Size), this, starSystemObject);
    //}

    private bool TryBuildMine(StarSystemObject starSystemObject)
    {
        //int x = Rnd.Next(starSystemObject.Size);
        //int y = Rnd.Next(starSystemObject.Size);
        //if (!starSystemObject.IsPlaceOccupied(x, y))

        var deposit = starSystemObject.Deposits.FirstOrDefault(d => !d.IsOccupied);
        if (deposit != null)
        {
            //Mine mine = new Mine(x, y, this, starSystemObject);
            Mine mine = new Mine(this, deposit);

            Program.Mutex.WaitOne();
            Universe.Mines.Add(mine);
            starSystemObject.GroundObjects.Add(mine);
            Program.Mutex.ReleaseMutex();

            return true;
        }
        return false;
    }

    //public void BuildTank(StarSystemObject starSystemObject)
    //{
    //    new Tank(rnd.Next(starSystemObject.Size), rnd.Next(starSystemObject.Size), this, starSystemObject);
    //}

    public void BuildFactory(StarSystemObject starSystemObject)
    {
        Factory.New(Rnd.Next(starSystemObject.Size), Rnd.Next(starSystemObject.Size), this, starSystemObject);
    }

    public bool TryBuildFactory(StarSystemObject starSystemObject)
    {
        int x = Rnd.Next(starSystemObject.Size);
        int y = Rnd.Next(starSystemObject.Size);
        if (!starSystemObject.IsPlaceOccupied(x, y))
        {
            Factory.New(x, y, this, starSystemObject);
            return true;
        }
        return false;
    }
}
using SoulUniverse.PlanetObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulUniverse.Program;
using static SoulUniverse.Enums;

namespace SoulUniverse
{
    internal class Fraction
    {
        public string Name { get; protected set; }

        public ConsoleColor Color { get; protected set; }
        //public FractionColor Color { get; protected set; }
        public List<GroundObject> Property { get; } = new();
        public List<StarSystemObject> Colonies { get; protected set; } = new();
        public Dictionary<ResourceName, int> Recources { get; } = new();

        //Фракция игрока
        public Fraction()
        {
            Name = "Игрок";
            Color = ConsoleColor.Green;
        }

        public Fraction(FractionName Fraction)
        {
            Name = Fraction.ToString();
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

        public void DoSomething()
        {
            if (IsEnoughToBuildMine())
            {
                BuildMine(this.Colonies.ElementAt(rnd.Next(Colonies.Count)));
                Debug.WriteLine(string.Format("Насекомые из {0} построили шахту", Name));
            }

            if (IsEnoughToBuildTank())
            {
                BuildTank(this.Colonies.ElementAt(rnd.Next(Colonies.Count)));
                Debug.WriteLine(string.Format("Насекомые из {0} построили ТАНК! Будет война", Name));
            }

            if (IsEnoughToBuildMilitaryBase())
            {
                BuildMilitaryBase(this.Colonies.ElementAt(rnd.Next(Colonies.Count)));
                Debug.WriteLine(string.Format("Насекомые из {0} построили базу", Name));
            }

            else Debug.WriteLine(string.Format("Насекомые из {0} обнищали", Name));
        }

        public bool IsEnoughToBuildMine()
        {
            foreach (var kvp in Recources)
            {
                if (kvp.Value < Mine.Cost.Find(k => k.Key == kvp.Key).Value)
                {
                    return false;
                }
            }
            return true;
        }

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

        //public void Build<T>(StarSystemObject starSystemObject, T groundObject) where T : GroundObject, new()
        //{
        //    starSystemObject.GroundObjects.Add(new T());

        //}

        public void BuildMilitaryBase(StarSystemObject starSystemObject)
        {
            foreach (var res in Recources)
            {
                Recources[res.Key] = res.Value - MilitaryBase.Cost.Find(r => r.Key == res.Key).Value;
            }
            mutex.WaitOne();
            starSystemObject.GroundObjects.Add(new MilitaryBase(rnd.Next(starSystemObject.Size), rnd.Next(starSystemObject.Size), this));
            mutex.ReleaseMutex();
        }

        public void BuildMine(StarSystemObject starSystemObject)
        {
            foreach (var res in Recources)
            {
                Recources[res.Key] = res.Value - Mine.Cost.Find(r => r.Key == res.Key).Value;
            }
            mutex.WaitOne();
            starSystemObject.GroundObjects.Add(new Mine(rnd.Next(starSystemObject.Size), rnd.Next(starSystemObject.Size), this, starSystemObject));
            mutex.ReleaseMutex();
        }

        public void BuildTank(StarSystemObject starSystemObject)
        {
            foreach (var res in Recources)
            {
                Recources[res.Key] = res.Value - Tank.Cost.Find(r => r.Key == res.Key).Value;
            }
                mutex.WaitOne();
            starSystemObject.GroundObjects.Add(new Tank(rnd.Next(starSystemObject.Size), rnd.Next(starSystemObject.Size), this, starSystemObject));
            mutex.ReleaseMutex();
        }
    }
}

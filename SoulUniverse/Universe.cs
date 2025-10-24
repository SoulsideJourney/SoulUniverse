using SoulUniverse.Objects.PlanetObjects;
using SoulUniverse.Objects.StarSystemObjects;
using SoulUniverse.Objects.VoidObjects;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse;

internal static class Universe
{
    /// <summary> Граница генерации мира по оси X </summary>
    public const int UniverseX = 100;

    /// <summary> Граница генерации мира по оси X </summary>
    public const int UniverseY = 40;

    /// <summary> Список объектов </summary>
    public static readonly List<VoidObject> VoidObjects = new();

    /// <summary> Шахты </summary>
    internal static readonly List<Mine> Mines = new();

    /// <summary> Танки </summary>
    internal static readonly List<Tank> Tanks = new();

    /// <summary> Заводы </summary>
    internal static readonly List<Factory> Factories = new();

    /// <summary> Список фракций </summary>
    internal static readonly List<Fraction> NpcFractions = new();

    private static DateTime _currentTime = DateTime.Today.Date;

    public static DateTime CurrentDate
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            SetConsoleTitle();
        }
    }

    //Ссылки на родной мир
    public static Star HomeStar = null!;
    public static Planet HomePlanet = null!;

    /// <summary> Создание всего сущего во Вселенной </summary>
    public static void Initialize()
    {
        //Создание фракций
        foreach (FractionName fraction in Enum.GetValues(typeof(FractionName)))
        {
            NpcFractions.Add(new Fraction(fraction));
        }

        //Фракция игрока
        var playerFraction = Fraction.CreatePlayerFraction();

        //Создание объектов
        GenerateObjects<Star>(100);
        GenerateObjects<Wormhole>(10);
        GenerateObjects<BlackHole>(10);

        //Нахождение рандомной планеты континентального типа и выбор её в качестве родной
        while (true)
        {
            if (VoidObjects.ElementAt(Rnd.Next(VoidObjects.Count)) is Star star && star.StarSystemObjects.Any(obj => obj is Planet { PlanetClass: PlanetClass.Continental }))
            {
                HomeStar = star;
                HomePlanet = (Planet)star.StarSystemObjects.First(obj => obj is Planet { PlanetClass: PlanetClass.Continental });
                HomePlanet.Fractions.Add(playerFraction);

                MilitaryBase.New(Rnd.Next(HomePlanet.Size), Rnd.Next(HomePlanet.Size), playerFraction, HomePlanet);
                break;
            }
        }

        //Добавляем фракции на планеты с небольшой долей вероятности
        foreach (VoidObject voidObject in VoidObjects)
        {
            if (voidObject is not Star star) continue;
            foreach (StarSystemObject starSystemObject in star.StarSystemObjects)
            {
                if (starSystemObject is not Planet planet) continue;
                while (true)
                {
                    if (Rnd.Next(100) < 10)
                    {
                        Fraction fraction = NpcFractions.ElementAt(Rnd.Next(NpcFractions.Count));
                        planet.Fractions.Add(fraction);
                        fraction.Colonies.Add(planet);
                    }
                    else break;
                }
            }
        }
    }

    private static void GenerateObjects<T>(int number) where T : VoidObject, new()
    {
        for (int i = 0; i < number; i++)
        {
            int x, y;

            while (true)
            {
                x = Rnd.Next(UniverseX);
                y = Rnd.Next(UniverseY);

                if (VoidObjects.Any(obj => obj.Coordinates.x == x && obj.Coordinates.y == y))
                {
                    continue;
                }
                T voidObject = new()
                {
                    Coordinates =
                    {
                        x = x,
                        y = y
                    }
                };
                VoidObjects.Add(voidObject);
                break;
            }
        }
    }
}
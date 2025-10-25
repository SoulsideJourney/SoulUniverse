using SoulUniverse.Objects.StarSystemObjects;
using static SoulUniverse.ConsoleHelper;
using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.Objects.VoidObjects;

internal class Star : VoidObject
{
    protected override char Symbol { get; } = '*';

    protected override ConsoleColor Color { get; }

    private readonly StarClass _starClass;

    public List<StarSystemObject> StarSystemObjects = [];

    public Star()
    {
        Coordinates.X = Rnd.Next(Universe.UniverseX);
        Coordinates.Y = Rnd.Next(Universe.UniverseY);
        _starClass = (StarClass)Rnd.Next(Enum.GetValues(typeof(StarClass)).Length - 1);
        Color = GetColor(_starClass);
        GenerateStarSystemObjects();
    }

    private void GenerateStarSystemObjects()
    {
        int objectCount = Rnd.Next(15);
        //Генерация объектов системы
        for (int i = 0; i < objectCount; i++)
        {
            bool isOrbitOccupied = false;
            while (!isOrbitOccupied)
            {
                int distance = Rnd.Next(2, Universe.UniverseY / 2 - 1);
                foreach (StarSystemObject obj in StarSystemObjects)
                {
                    if (obj.Distance == distance)
                    {
                        isOrbitOccupied = true;
                        break;
                    }
                }
                if (isOrbitOccupied) continue;

                //Орбита свободна -- добавляем планету или пояс астероидов
                var rnd = Rnd.Next(100);
                if (rnd < 70)
                {
                    Planet planet = new(distance);
                    //Добавляем фракцию на планету с небольшой долей вероятности
                    while (true)
                    {
                        if (Rnd.Next(100) < 10)
                        {
                            Fraction fraction = Universe.NpcFractions.ElementAt(Rnd.Next(Universe.NpcFractions.Count));
                            planet.Fractions.Add(fraction);
                            fraction.Colonies.Add(planet);
                        }
                        else break;
                    }
                    StarSystemObjects.Add(planet);
                }
                else
                {
                    //Создаем несколько астероидов в поясе в зависимости от радиуса орбиты
                    for (int j = 0; j < 10; j++)
                    {
                        StarSystemObjects.Add(new Asteroid(distance));
                    }
                }

                break;
            }
        }
        StarSystemObjects.Sort();
    }

    public override void Draw()
    {
        Draw(Coordinates.X, Coordinates.Y);
    }

    public void Draw(int x, int y)
    {
        lock (Locker)
        {
            if (FractionDisplayMode == DisplayMode.Fractions)
            {
                //Фракционный цвет звезды по цвету первой попавшейся фракции в системе окда?
                //FractionColor? fractionColor = starSystemObjects.Find(obj => obj.Fractions.Count > 0)?.Fractions.ElementAt(0).Color;
                //ConsoleColor? temp = (ConsoleColor)fractionColor;
                Console.ForegroundColor = StarSystemObjects.Find(obj => obj.Fractions.Count > 0)?.Fractions.ElementAt(0).Color ?? ConsoleColor.Gray;
                //Console.ForegroundColor = temp ?? ConsoleColor.DarkGray;
            }
            else
            {
                Console.ForegroundColor = GetColor();
            }
            Console.SetCursorPosition(x, y);
            Console.Write("*");
        }
    }

    public void DrawStarSystemObjects()
    {
        lock (Locker)
        {
            Draw(20, 20);
            foreach (StarSystemObject starSystemObject in StarSystemObjects)
            {
                starSystemObject.Draw();
            }
        }
    }

    public void WriteStarInfo()
    {
        lock (Locker)
        {
            int row = 2;
            string starClass = _starClass.ToString();
            Console.Write("Звезда класса ");
            Console.ForegroundColor = GetColor();
            Console.Write(starClass);
            ResetConsoleColor();
            Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
            int planets = StarSystemObjects.Count;
            Console.Write($"Количество планетарных тел: {planets}");

            //Информация о планетах
            if (planets > 0)
            {
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write("Информация об объектах системы:");
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write("-----------------------------------------");
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write($"| № | Расстояние | {"Класс",11} | Размер |");
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write("-----------------------------------------");
                int planetNumber = 0;
                foreach (StarSystemObject starSystemObject in StarSystemObjects)
                {
                    Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                    //string planetClass;
                    if (starSystemObject is Planet planet)
                    {
                        //planetClass = planet.PlanetClass.ToString();
                        Console.Write($"|{++planetNumber,2} | {starSystemObject.Distance,6} а.е | ");
                        Console.ForegroundColor = planet.Color;
                        Console.Write($"{planet.PlanetClass,11}");
                        ResetConsoleColor();
                        Console.Write($" | {starSystemObject.Size,6} |");
                    }
                }
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                Console.Write("-----------------------------------------");

                //Информация о присутствующих фракциях
                Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                List<Fraction> fractionsInSystem = new List<Fraction>();
                foreach (StarSystemObject starSystemObject in StarSystemObjects)
                {
                    foreach (Fraction fraction in starSystemObject.Fractions)
                    {
                        if (!fractionsInSystem.Contains(fraction)) fractionsInSystem.Add(fraction);
                    }
                }
                if (fractionsInSystem.Count > 0)
                {
                    Console.Write("Присутствующие фракции в системе:");
                    foreach (Fraction fraction in fractionsInSystem)
                    {
                        Console.SetCursorPosition(Universe.UniverseX + 2, ++row);
                        Console.ForegroundColor = fraction.Color;
                        Console.Write(fraction.Name);
                    }
                    ResetConsoleColor();
                }
                else Console.Write("Нет присутствующих фракций");
            }

            InfoIsClear = false;

            //Возвращение курсора
            Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
        }
    }

    private ConsoleColor GetColor()
    {
        return GetColor(_starClass);
    }

    private ConsoleColor GetColor(StarClass starClass)
    {
        return starClass switch
        {
            StarClass.W => ConsoleColor.Magenta,
            StarClass.O => ConsoleColor.DarkBlue,
            StarClass.B => ConsoleColor.Blue,
            StarClass.A => ConsoleColor.DarkCyan,
            StarClass.F => ConsoleColor.White,
            StarClass.G => ConsoleColor.Yellow,
            StarClass.K => ConsoleColor.DarkYellow,
            StarClass.M => ConsoleColor.Red,
            StarClass.L => ConsoleColor.DarkRed,
            _ => ConsoleColor.White
        };
    }

    public enum StarClass
    {
        W, O, B, A, F, G, K, M, L
    }
}
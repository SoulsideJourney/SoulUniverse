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
            bool isPositionOccupied = false;
            while (!isPositionOccupied)
            {
                int distance = Rnd.Next(2, Universe.UniverseY / 2 - 1);
                foreach (StarSystemObject obj in StarSystemObjects)
                {
                    if (obj.Distance == distance)
                    {
                        isPositionOccupied = true;
                        break;
                    }
                }
                if (isPositionOccupied) continue;

                //Орбита свободна -- добавляем планету
                Planet planet = new(distance);
                Asteroid asteroid = new(distance); //TODO
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
                break;
            }
        }
        StarSystemObjects.Sort();
    }

    public override void Draw()
    {
        Draw(Coordinates.X, Coordinates.Y, _starClass);
    }

    public void Draw(int x, int y)
    {
        Draw(x, y, _starClass);
    }

    protected void Draw(int x, int y, StarClass starClass)
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
            SetCursor(x, y);
            Write("*");
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
            Write("Звезда класса ");
            Console.ForegroundColor = GetColor();
            Write(starClass);
            ResetConsoleColor();
            SetCursor(Universe.UniverseX + 2, ++row);
            int planets = StarSystemObjects.Count;
            Write($"Количество планетарных тел: {planets}");

            //Информация о планетах
            if (planets > 0)
            {
                SetCursor(Universe.UniverseX + 2, ++row);
                Write("Информация об объектах системы:");
                SetCursor(Universe.UniverseX + 2, ++row);
                Write("-----------------------------------------");
                SetCursor(Universe.UniverseX + 2, ++row);
                Write($"| № | Расстояние | {"Класс",11} | Размер |");
                SetCursor(Universe.UniverseX + 2, ++row);
                Write("-----------------------------------------");
                int planetNumber = 0;
                foreach (StarSystemObject starSystemObject in StarSystemObjects)
                {
                    SetCursor(Universe.UniverseX + 2, ++row);
                    //string planetClass;
                    if (starSystemObject is Planet planet)
                    {
                        //planetClass = planet.PlanetClass.ToString();
                        Write($"|{++planetNumber,2} | {starSystemObject.Distance,6} а.е | ");
                        Console.ForegroundColor = planet.Color;
                        Write($"{planet.PlanetClass,11}");
                        ResetConsoleColor();
                        Write($" | {starSystemObject.Size,6} |");
                    }
                }
                SetCursor(Universe.UniverseX + 2, ++row);
                Write("-----------------------------------------");

                //Информация о присутствующих фракциях
                SetCursor(Universe.UniverseX + 2, ++row);
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
                    Write("Присутствующие фракции в системе:");
                    foreach (Fraction fraction in fractionsInSystem)
                    {
                        SetCursor(Universe.UniverseX + 2, ++row);
                        Console.ForegroundColor = fraction.Color;
                        Write(fraction.Name);
                    }
                    ResetConsoleColor();
                }
                else Write("Нет присутствующих фракций");
            }

            InfoIsClear = false;

            //Возвращение курсора
            SetCursor(CurrentCursorX, CurrentCursorY);
        }
    }

    private ConsoleColor GetColor()
    {
        return GetColor(_starClass);
    }
        
    private static ConsoleColor GetColor(StarClass starClass)
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
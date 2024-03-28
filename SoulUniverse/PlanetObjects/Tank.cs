using static SoulUniverse.Enums;
using static SoulUniverse.Program;

namespace SoulUniverse.PlanetObjects
{
    internal class Tank : GroundProperty, IMovable
    {
        //public override Fraction Owner { get; }

        //public override StarSystemObject Location { get; }

        protected override char Symbol => '♣';

        protected override string Name => "Танк мать его";

        static public List<KeyValuePair<ResourceName, int>> Cost { get; } = new()
        {
            new KeyValuePair<ResourceName, int>(ResourceName.Iron, 10),
            new KeyValuePair<ResourceName, int>(ResourceName.Uranium, 1),
            new KeyValuePair<ResourceName, int>(ResourceName.Oil, 5),
        };

        public bool IsNeedToRedraw { get; set; } = true;
        public Coordinates DrawedCoordinates { get; set; } = new();

        public Tank(int x, int y, Fraction fraction, StarSystemObject starSystemObject) : base(x, y, fraction, starSystemObject)
        {
            foreach (var res in fraction.Recources)
            {
                fraction.Recources[res.Key] = res.Value - Tank.Cost.Find(r => r.Key == res.Key).Value;
            }

            mutex.WaitOne();
            Universe.Tanks.Add(this);
            starSystemObject.GroundObjects.Add(this);
            mutex.ReleaseMutex();
        }

        //public Tank(int x, int y, Fraction fraction, StarSystemObject starSystemObject)
        //{
        //    Coordinates.x = x;
        //    Coordinates.y = y;
        //    Owner = fraction;
        //    Location = starSystemObject;
        //    Program.tanks.Add(this);
        //}

        /// <summary>Рандомное движение танка на одно поле</summary>
        public void Move()
        {
            if (DrawedCoordinates.x != 0 && DrawedCoordinates.y != 0)
            {

            }
            int x = Coordinates.x;
            int y = Coordinates.y;
            _ = new Random().Next(4) switch
            {
                0 => x < Location.Size ? x += 1 : x -= 1,
                1 => x > 0 ? x -= 1 : x += 1,
                2 => y < Location.Size ? y += 1 : y -= 1,
                _ => y > 0 ? y -= 1 : y += 1,
            };

            if (!(Location as Planet).IsPlaceOccupied(x, y))
            {
                Coordinates.x = x;
                Coordinates.y = y;
                IsNeedToRedraw = true;
            }
        }

        public override void Draw()
        {
            lock (locker)
            {
                Console.SetCursorPosition(Coordinates.x, Coordinates.y);
                if (FractionDisplayMode == Enums.DisplayMode.Fractions) Console.ForegroundColor = Owner.Color;
                else ResetConsoleColor();
                Console.Write(Symbol);
                DrawedCoordinates.x = Coordinates.x;
                DrawedCoordinates.y = Coordinates.y;
                ResetConsoleColor();
                Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
                IsNeedToRedraw = false;
            }
        }

        public void Erase()
        {
            lock (locker)
            {
                Console.SetCursorPosition(DrawedCoordinates.x, DrawedCoordinates.y);
                Console.Write(' ');
                Console.SetCursorPosition(CurrentCursorX, CurrentCursorY);
            }
        }
    }
}

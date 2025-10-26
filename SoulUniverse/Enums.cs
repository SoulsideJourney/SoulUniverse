namespace SoulUniverse;

public class Enums
{
    public enum FractionColor
    {
        //Black = ConsoleColor.Black,
        DarkBlue = ConsoleColor.DarkBlue,
        DarkGreen = ConsoleColor.DarkGreen,
        DarkCyan = ConsoleColor.DarkCyan,
        DarkRed = ConsoleColor.DarkRed,
        DarkMagenta = ConsoleColor.DarkMagenta,
        DarkYellow = ConsoleColor.DarkYellow,
        //Gray = ConsoleColor.Gray,
        DarkGray = ConsoleColor.DarkGray,
        Blue = ConsoleColor.Blue,
        //Green = ConsoleColor.Green,
        Cyan = ConsoleColor.Cyan,
        Red = ConsoleColor.Red,
        Magenta = ConsoleColor.Magenta,
        Yellow = ConsoleColor.Yellow,
        White = ConsoleColor.White
    }

    public enum DisplayMode
    {
        Types, Fractions
    }

    public enum Scale
    {
        Universe, StarSystem, Planet //TODO режим отображения окресностей планеты со спутниками
    }

    public enum FractionName
    {
        Джедаи, КосмическиеБомжи, Пираты, Рептилоиды, Ситхи, ТретьяАкадемия,
        Бабки_с_РПГ, РогатыеТрупоеды, КакаяТоЖуткаяХуйня, ЧетвертыйРейх, РоботыЭкстерминаторы,
        Салат, ФеечкиВинкс, СейлорМун, ОжившаяПротухшаяЕдаИзХолодильника, Плесень,
        Далеки, Империя, Русичи
    }

    public enum PlanetClass
    {
        Barren, Continental, GasGiant, Hycean
    }

    public enum ResourceName
    {
        Iron, Oil, Uranium,
    }
}
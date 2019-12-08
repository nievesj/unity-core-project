namespace Core.Common.Extensions.String
{
    public static class StringLoggingExtensions
    {
        /// <summary>
        /// Sets the color of the text according to the parameter value.
        /// </summary>
        /// <param name="message"> Message. </param>
        /// <param name="color">   Color. </param>
        public static string Colored(this string message, Colors color)
        {
#if UNITY_EDITOR
            return $"<color={color}>{message}</color>";
#else
		return message;
#endif
        }

        /// <summary>
        /// Sets the color of the text according to the traditional HTML format parameter value.
        /// </summary>
        /// <param name="message"> Message </param>
        /// <param name="colorCode"></param>
        public static string Colored(this string message, string colorCode)
        {
#if UNITY_EDITOR
            return $"<color={colorCode}>{message}</color>";
#else
		return message;
#endif
        }

        /// <summary>
        /// Sets the size of the text according to the parameter value, given in pixels.
        /// </summary>
        /// <param name="message"> Message. </param>
        /// <param name="size">    Size. </param>
        public static string Sized(this string message, int size)
        {
#if UNITY_EDITOR
            return $"<size={size}>{message}</size>";
#else
		return message;
#endif
        }

        /// <summary>
        /// Renders the text in boldface.
        /// </summary>
        /// <param name="message"> Message. </param>
        public static string Bold(this string message)
        {
#if UNITY_EDITOR
            return $"<b>{message}</b>";
#else
		return message;
#endif
        }

        /// <summary>
        /// Renders the text in italics.
        /// </summary>
        /// <param name="message"> Message. </param>
        public static string Italics(this string message)
        {
#if UNITY_EDITOR
            return $"<i>{message}</i>";
#else
		return message;
#endif
        }
    }

    public struct Colors
    {
        public const string DarkBlue = "#00008B";
        public const string MediumBlue = "#0000CD";
        public const string Blue = "#0000FF";
        public const string DarkGreen = "#006400";
        public const string Green = "#008000";
        public const string Teal = "#008080";
        public const string DarkCyan = "#008B8B";
        public const string DeepSkyBlue = "#00BFFF";
        public const string DarkTurquoise = "#00CED1";
        public const string MediumSpringGreen = "#00FA9A";
        public const string Lime = "#00FF00";
        public const string SpringGreen = "#00FF7F";
        public const string Aqua = "#00FFFF";
        public const string Cyan = "#00FFFF";
        public const string MidnightBlue = "#191970";
        public const string DodgerBlue = "#1E90FF";
        public const string LightSeaGreen = "#20B2AA";
        public const string ForestGreen = "#228B22";
        public const string SeaGreen = "#2E8B57";
        public const string DarkSlateGray = "#2F4F4F";
        public const string DarkSlateGrey = "#2F4F4F";
        public const string LimeGreen = "#32CD32";
        public const string MediumSeaGreen = "#3CB371";
        public const string Turquoise = "#40E0D0";
        public const string RoyalBlue = "#4169E1";
        public const string SteelBlue = "#4682B4";
        public const string DarkSlateBlue = "#483D8B";
        public const string MediumTurquoise = "#48D1CC";
        public const string Indigo = "#4B0082";
        public const string DarkOliveGreen = "#556B2F";
        public const string CadetBlue = "#5F9EA0";
        public const string CornflowerBlue = "#6495ED";
        public const string RebeccaPurple = "#663399";
        public const string MediumAquaMarine = "#66CDAA";
        public const string DimGray = "#696969";
        public const string DimGrey = "#696969";
        public const string SlateBlue = "#6A5ACD";
        public const string OliveDrab = "#6B8E23";
        public const string SlateGray = "#708090";
        public const string SlateGrey = "#708090";
        public const string LightSlateGray = "#778899";
        public const string LightSlateGrey = "#778899";
        public const string MediumSlateBlue = "#7B68EE";
        public const string LawnGreen = "#7CFC00";
        public const string Chartreuse = "#7FFF00";
        public const string Aquamarine = "#7FFFD4";
        public const string Maroon = "#800000";
        public const string Purple = "#800080";
        public const string Olive = "#808000";
        public const string Gray = "#808080";
        public const string Grey = "#808080";
        public const string SkyBlue = "#87CEEB";
        public const string LightSkyBlue = "#87CEFA";
        public const string BlueViolet = "#8A2BE2";
        public const string DarkRed = "#8B0000";
        public const string DarkMagenta = "#8B008B";
        public const string SaddleBrown = "#8B4513";
        public const string DarkSeaGreen = "#8FBC8F";
        public const string LightGreen = "#90EE90";
        public const string MediumPurple = "#9370DB";
        public const string DarkViolet = "#9400D3";
        public const string PaleGreen = "#98FB98";
        public const string DarkOrchid = "#9932CC";
        public const string YellowGreen = "#9ACD32";
        public const string Sienna = "#A0522D";
        public const string Brown = "#A52A2A";
        public const string DarkGray = "#A9A9A9";
        public const string DarkGrey = "#A9A9A9";
        public const string LightBlue = "#ADD8E6";
        public const string GreenYellow = "#ADFF2F";
        public const string PaleTurquoise = "#AFEEEE";
        public const string LightSteelBlue = "#B0C4DE";
        public const string PowderBlue = "#B0E0E6";
        public const string FireBrick = "#B22222";
        public const string DarkGoldenRod = "#B8860B";
        public const string MediumOrchid = "#BA55D3";
        public const string RosyBrown = "#BC8F8F";
        public const string DarkKhaki = "#BDB76B";
        public const string Silver = "#C0C0C0";
        public const string MediumVioletRed = "#C71585";
        public const string IndianRed = "#CD5C5C";
        public const string Peru = "#CD853F";
        public const string Chocolate = "#D2691E";
        public const string Tan = "#D2B48C";
        public const string LightGray = "#D3D3D3";
        public const string LightGrey = "#D3D3D3";
        public const string Thistle = "#D8BFD8";
        public const string Orchid = "#DA70D6";
        public const string GoldenRod = "#DAA520";
        public const string PaleVioletRed = "#DB7093";
        public const string Crimson = "#DC143C";
        public const string Gainsboro = "#DCDCDC";
        public const string Plum = "#DDA0DD";
        public const string BurlyWood = "#DEB887";
        public const string LightCyan = "#E0FFFF";
        public const string Lavender = "#E6E6FA";
        public const string DarkSalmon = "#E9967A";
        public const string Violet = "#EE82EE";
        public const string PaleGoldenRod = "#EEE8AA";
        public const string LightCoral = "#F08080";
        public const string Khaki = "#F0E68C";
        public const string AliceBlue = "#F0F8FF";
        public const string HoneyDew = "#F0FFF0";
        public const string Azure = "#F0FFFF";
        public const string SandyBrown = "#F4A460";
        public const string Wheat = "#F5DEB3";
        public const string Beige = "#F5F5DC";
        public const string WhiteSmoke = "#F5F5F5";
        public const string MintCream = "#F5FFFA";
        public const string GhostWhite = "#F8F8FF";
        public const string Salmon = "#FA8072";
        public const string AntiqueWhite = "#FAEBD7";
        public const string Linen = "#FAF0E6";
        public const string LightGoldenRodYellow = "#FAFAD2";
        public const string OldLace = "#FDF5E6";
        public const string Red = "#FF0000";
        public const string Fuchsia = "#FF00FF";
        public const string Magenta = "#FF00FF";
        public const string DeepPink = "#FF1493";
        public const string OrangeRed = "#FF4500";
        public const string Tomato = "#FF6347";
        public const string HotPink = "#FF69B4";
        public const string Coral = "#FF7F50";
        public const string DarkOrange = "#FF8C00";
        public const string LightSalmon = "#FFA07A";
        public const string Orange = "#FFA500";
        public const string LightPink = "#FFB6C1";
        public const string Pink = "#FFC0CB";
        public const string Gold = "#FFD700";
        public const string PeachPuff = "#FFDAB9";
        public const string NavajoWhite = "#FFDEAD";
        public const string Moccasin = "#FFE4B5";
        public const string Bisque = "#FFE4C4";
        public const string MistyRose = "#FFE4E1";
        public const string BlanchedAlmond = "#FFEBCD";
        public const string PapayaWhip = "#FFEFD5";
        public const string LavenderBlush = "#FFF0F5";
        public const string SeaShell = "#FFF5EE";
        public const string Cornsilk = "#FFF8DC";
        public const string LemonChiffon = "#FFFACD";
        public const string FloralWhite = "#FFFAF0";
        public const string Snow = "#FFFAFA";
        public const string Yellow = "#FFFF00";
        public const string LightYellow = "#FFFFE0";
        public const string Ivory = "#FFFFF0";
        public const string White = "#FFFFFF";
    }
}
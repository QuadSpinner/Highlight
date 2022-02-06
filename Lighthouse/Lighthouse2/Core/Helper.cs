using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Media;

namespace Lighthouse2.Core
{
    public static class Helper
    {
        public static char[] escapes = { ' ', '!', '"', '@', '$', '(', ')', '[', ']', '*', '-', '.', '/', '>', '<', '"', ':', ';', ',', '?', '\'', '\n', '\r', '\t', '=' };

        public static List<Color> colors = new();

        public static string LastSolution;

        public static Color Undefined { get; } = Color.FromArgb(0, 0, 0, 0);

        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
        public static Color ChangeAlpha(this Color c, byte alpha) => Color.FromArgb(alpha, c.R, c.G, c.B);

        public static string ColorToHex(this Color color) => $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

        public static List<Color> SetupColors(this List<Color> colors)
        {
            colors.Clear();
            colors.Add(HexToColor("#e4603c"));
            colors.Add(HexToColor("#efa48f"));
            colors.Add(HexToColor("#e4823c"));
            colors.Add(HexToColor("#efb78f"));
            colors.Add(HexToColor("#e4943c"));
            colors.Add(HexToColor("#efc28f"));
            colors.Add(HexToColor("#e4a83c"));
            colors.Add(HexToColor("#efcd8f"));
            colors.Add(HexToColor("#e4be3c"));
            colors.Add(HexToColor("#efda8f"));
            colors.Add(HexToColor("#e4d43c"));
            colors.Add(HexToColor("#efe68f"));
            colors.Add(HexToColor("#dde43c"));
            colors.Add(HexToColor("#ebef8f"));
            colors.Add(HexToColor("#bde43d"));
            colors.Add(HexToColor("#d9ef8f"));
            colors.Add(HexToColor("#8ce43c"));
            colors.Add(HexToColor("#bdef8f"));
            colors.Add(HexToColor("#4ee43c"));
            colors.Add(HexToColor("#99f08f"));
            colors.Add(HexToColor("#3de4a5"));
            colors.Add(HexToColor("#8fefcb"));
            colors.Add(HexToColor("#3dbde3"));
            colors.Add(HexToColor("#8fd9ef"));
            colors.Add(HexToColor("#3ca1e4"));
            colors.Add(HexToColor("#8fc9f0"));
            colors.Add(HexToColor("#3d7be3"));
            colors.Add(HexToColor("#8fb3ef"));
            colors.Add(HexToColor("#3c50e4"));
            colors.Add(HexToColor("#8f9bef"));
            colors.Add(HexToColor("#6d3ce4"));
            colors.Add(HexToColor("#ab8ff0"));
            colors.Add(HexToColor("#b53de4"));
            colors.Add(HexToColor("#d58fef"));
            colors.Add(HexToColor("#e43cc6"));
            colors.Add(HexToColor("#ef8fde"));
            colors.Add(HexToColor("#e43c80"));
            colors.Add(HexToColor("#ef8fb6"));
            colors.Add(HexToColor("#e43d41"));
            colors.Add(HexToColor("#ef8f92"));

            // <!-- /// -->
            colors.Add(HexToColor("#00a9fa"));
            colors.Add(HexToColor("#00bda4"));
            colors.Add(HexToColor("#0f0f0f"));
            colors.Add(HexToColor("#1683b1"));
            colors.Add(HexToColor("#20ac9a"));
            colors.Add(HexToColor("#218a7c"));
            colors.Add(HexToColor("#2a7172"));
            colors.Add(HexToColor("#363636"));
            colors.Add(HexToColor("#376c99"));
            colors.Add(HexToColor("#4a5c9c"));
            colors.Add(HexToColor("#4d7e63"));
            colors.Add(HexToColor("#4d8f6c"));
            colors.Add(HexToColor("#52a779"));
            colors.Add(HexToColor("#544b59"));
            colors.Add(HexToColor("#575276"));
            colors.Add(HexToColor("#589ccc"));
            colors.Add(HexToColor("#635a96"));
            colors.Add(HexToColor("#6a6a6a"));
            colors.Add(HexToColor("#6db570"));
            colors.Add(HexToColor("#6e8a59"));
            colors.Add(HexToColor("#86b065"));
            colors.Add(HexToColor("#905986"));
            colors.Add(HexToColor("#9482a9"));
            colors.Add(HexToColor("#949494"));
            colors.Add(HexToColor("#958e60"));
            colors.Add(HexToColor("#977723"));
            colors.Add(HexToColor("#97d9ad"));
            colors.Add(HexToColor("#98a1a9"));
            colors.Add(HexToColor("#9b576d"));
            colors.Add(HexToColor("#9b6657"));
            colors.Add(HexToColor("#a5b1d9"));
            colors.Add(HexToColor("#ad9559"));
            colors.Add(HexToColor("#afafaf"));
            colors.Add(HexToColor("#b0d6e0"));
            colors.Add(HexToColor("#bc5e8d"));
            colors.Add(HexToColor("#c96395"));
            colors.Add(HexToColor("#cc9863"));
            colors.Add(HexToColor("#ccb4d6"));
            colors.Add(HexToColor("#cdaef1"));
            colors.Add(HexToColor("#cdc172"));
            colors.Add(HexToColor("#d6d6d6"));
            colors.Add(HexToColor("#dbba6a"));
            colors.Add(HexToColor("#df6475"));
            colors.Add(HexToColor("#e89e94"));
            colors.Add(HexToColor("#f1f1f1"));
            colors.Add(HexToColor("#f4d6a3"));
            colors.Add(HexToColor("#f6d973"));
            colors.Add(HexToColor("#fefefe"));

            return colors;
        }

        public static Color HexToColor(string value)
        {
            value = value.Trim('#');
            switch (value.Length)
            {
                case 0:
                    return Undefined;
                case <= 6:
                    value = "FF" + value.PadLeft(6, '0');
                    break;
            }

            return uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint u)
                ? UIntToColor(u)
                : Undefined;
        }

        public static Color UIntToColor(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
        public static Color GetRandomColor()
        {
            if (colors.Count == 0)
                colors.SetupColors();

            Random r = new(Environment.TickCount);
            return colors[r.Next(0, colors.Count - 1)];
        }

        public static void InitDefaults()
        {
            colors.SetupColors();
        }

        internal static HighlightTag[] GetFillerTags()
        {
            return
                new HighlightTag[]
                {
                    new()
                    {
                        Color = Colors.Aquamarine,
                        Criteria = "~~1",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.BlueViolet,
                        Criteria = "~~2",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.Firebrick,
                        Criteria = "~~3",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.SkyBlue,
                        Criteria = "~~4",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.Orange,
                        Criteria = "~~5",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.GreenYellow,
                        Criteria = "~~6",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.DeepSkyBlue,
                        Criteria = "~~7",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.PeachPuff,
                        Criteria = "~~8",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.CadetBlue,
                        Criteria = "~~9",
                        Shape = TagShape.Line
                    },
                    new()
                    {
                        Color = Colors.LightSeaGreen,
                        Criteria = "~~0",
                        Shape = TagShape.Line
                    }
                };
        }

    }
}


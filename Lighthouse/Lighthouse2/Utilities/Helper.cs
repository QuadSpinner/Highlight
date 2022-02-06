using System.Collections.Generic;
using System.Windows.Media;
using Lighthouse2.Highlights;

namespace Lighthouse2.Utilities
{
    public static class Helper
    {
        public static char[] escapes = { ' ', '!', '"', '@', '$', '(', ')', '[', ']', '*', '-', '.', '/', '>', '<', '"', ':', ';', ',', '?', '\'', '\n', '\r', '\t', '=' };

        public static List<Color> colors = new();

        public static Dictionary<string, HighlightTag> keywordFormats = new();

        public static void InitDefaults()
        {
            colors.SetupColors();

            if (!keywordFormats.ContainsKey("Lighthouse"))
                keywordFormats.Add("Lighthouse", new HighlightTag()
                {
                    Shape = TagShape.LineUnder,
                    Color = Color.FromRgb(255, 200, 20),
                    Blur = BlurIntensity.None,
                    Criteria = "Lighthouse"
                });
        }

        public static void Load()
        {
        }
    }
}
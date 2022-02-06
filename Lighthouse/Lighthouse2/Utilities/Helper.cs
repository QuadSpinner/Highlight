using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;
using Lighthouse2.Highlights;

namespace Lighthouse2.Utilities
{
    public static class Helper
    {
        public static char[] escapes = {' ', '!', '"', '@', '$', '(', ')', '[', ']', '*', '-', '.', '/', '>', '<', '"', ':', ';', ',', '?', '\'', '\n', '\r', '\t', '='};

        public static List<Color> colors = new();

        public static Dictionary<string, HighlightTag> keywordFormats = new();

        public static void InitDefaults()
        {
            colors.SetupColors();

            if (keywordFormats.Count == 0)
            {
                keywordFormats.Add("Lighthouse", new HighlightTag
                {
                    Shape = TagShape.Tag,
                    Color = Color.FromRgb(255, 200, 20),
                    Blur = BlurIntensity.None,
                    Criteria = "Lighthouse"
                });
                keywordFormats.Add("Gaea", new HighlightTag
                {
                    Shape = TagShape.TagUnder,
                    Color = Color.FromRgb(81, 212, 240),
                    Blur = BlurIntensity.High,
                    Criteria = "Gaea"
                });
                keywordFormats.Add("Erosion", new HighlightTag
                {
                    Shape = TagShape.Line,
                    Color = Color.FromRgb(112, 255, 114),
                    Blur = BlurIntensity.Medium,
                    Criteria = "Erosion"
                });
                keywordFormats.Add("Build", new HighlightTag
                {
                    Shape = TagShape.LineUnder,
                    Color = Color.FromRgb(112, 255, 114),
                    Blur = BlurIntensity.Ultra,
                    Criteria = "Build"
                });
            }
        }


        internal static List<HighlightTag> LoadTagsFromFile(string file)
        {
            List<HighlightTag> returnValue;

            try
            {
                returnValue = (List<HighlightTag>) new XmlSerializer(typeof(List<HighlightTag>))
                    .Deserialize(new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
            }
            catch (Exception)
            {
                // Load defaults
                returnValue =
                    new List<HighlightTag>
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

            return returnValue;
        }

        internal static void SaveTagsToFile(string file, List<HighlightTag> HighlightTags)
        {
            new XmlSerializer(typeof(List<HighlightTag>))
                .Serialize(new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite), HighlightTags);
        }

        internal static void SaveSettingsToFile(string file, LighthouseOptions HighlightTags)
        {
            new XmlSerializer(typeof(LighthouseOptions))
                .Serialize(new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite), HighlightTags);
        }
    }
}
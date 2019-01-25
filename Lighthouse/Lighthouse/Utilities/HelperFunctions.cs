using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Lighthouse.Utilities
{
    internal static class HelperFunctions
    {
        internal static List<ColorTag> LoadTagsFromFile(string file)
        {
            List<ColorTag> returnValue;

            try
            {
                returnValue = (List<ColorTag>)new XmlSerializer(typeof(List<ColorTag>))
                    .Deserialize(new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
            }
            catch (Exception)
            {
                // Load defaults
                returnValue =
                new List<ColorTag>
                {
                    new ColorTag
                    {
                        ColorSwatch = Colors.Aquamarine,
                        Criteria = "~~1",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.BlueViolet,
                        Criteria = "~~2",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.Firebrick,
                        Criteria = "~~3",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.SkyBlue,
                        Criteria = "~~4",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.Orange,
                        Criteria = "~~5",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.GreenYellow,
                        Criteria = "~~6",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.DeepSkyBlue,
                        Criteria = "~~7",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.PeachPuff,
                        Criteria = "~~8",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.CadetBlue,
                        Criteria = "~~9",
                        isFullLine = true
                    },
                    new ColorTag
                    {
                        ColorSwatch = Colors.LightSeaGreen,
                        Criteria = "~~0",
                        isFullLine = true
                    }
                };
            }
            return returnValue;
        }

        internal static void SaveTagsToFile(string file, List<ColorTag> ColorTags)
        {
            new XmlSerializer(typeof(List<ColorTag>))
                .Serialize(new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite), ColorTags);
        }
        internal static void SaveSettingsToFile(string file, LighthouseOptions ColorTags)
        {
            new XmlSerializer(typeof(LighthouseOptions))
                .Serialize(new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite), ColorTags);
        }

        internal static void WriteOnOutputWindow(IVsOutputWindow provider, string text)
        {
            if (null == provider)
            {
                return;
            }

            IVsOutputWindow outputWindow = provider;

            Guid guidGeneral = VSConstants.GUID_OutWindowGeneralPane;
            if (ErrorHandler.Failed(outputWindow.GetPane(ref guidGeneral, out IVsOutputWindowPane windowPane)) ||
                null == windowPane)
            {
                ErrorHandler.Failed(outputWindow.CreatePane(ref guidGeneral, "General", 1, 0));
                if (ErrorHandler.Failed(outputWindow.GetPane(ref guidGeneral, out windowPane)) ||
                null == windowPane)
                {
                    return;
                }
                ErrorHandler.Failed(windowPane.Activate());
            }

            if (ErrorHandler.Failed(windowPane.OutputString(text)))
            {
            }
        }
    }
}
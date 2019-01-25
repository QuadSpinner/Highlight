using Lighthouse.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Lighthouse
{
    public class LighthouseOptions
    {
        public const string settingsFile = "Lighthouse.xml";
        public const string tagsFile = "CommonTags.xml";

        public static readonly string localPath =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\QuadSpinner\\VSX\\";

        public LighthouseOptions() => Directory.CreateDirectory(localPath);

        [Category("Highlighting")]
        [DisplayName("Color Styles")]
        [XmlIgnore]
        public List<ColorTag> ColorTags { get; set; }

        [DefaultValue(0)]
        [Category("Highlighting")]
        public BlurIntensity Blur { get; set; } = BlurIntensity.Low;

        [Category("Highlighting")]
        public BlurType Blurred { get; set; }

        [Category("Highlighting")]
        public CornerStyle HighlightCorner { get; set; }

        [Category("Highlighting")]
        [DisplayName("Override Styles")]
        public bool OverrideStyles { get; set; } = true;

        public void SaveSettingsToStorage()
        {
            Directory.CreateDirectory(localPath);
            HelperFunctions.SaveTagsToFile(localPath + tagsFile, ColorTags);
            HelperFunctions.SaveSettingsToFile(localPath + settingsFile, this);
        }

        public void LoadSettingsFromStorage()
        {
            try
            {
                ColorTags = HelperFunctions.LoadTagsFromFile(localPath + tagsFile);
            }
            catch (Exception) { }
        }
    }
}